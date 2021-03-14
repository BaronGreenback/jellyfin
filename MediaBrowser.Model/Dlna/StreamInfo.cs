#nullable disable
#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Session;

namespace MediaBrowser.Model.Dlna
{
    /// <summary>
    /// Class StreamInfo.
    /// </summary>
    public class StreamInfo
    {
        public StreamInfo()
        {
            AudioCodecs = Array.Empty<string>();
            VideoCodecs = Array.Empty<string>();
            SubtitleCodecs = Array.Empty<string>();
            TranscodeReasons = Array.Empty<TranscodeReason>();
            StreamOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public Guid ItemId { get; set; }

        public PlayMethod PlayMethod { get; set; }

        public EncodingContext Context { get; set; }

        public DlnaProfileType MediaType { get; set; }

        public string Container { get; set; }

        public string SubProtocol { get; set; }

        public long StartPositionTicks { get; set; }

        public int? SegmentLength { get; set; }

        public int? MinSegments { get; set; }

        public bool BreakOnNonKeyFrames { get; set; }

        public bool RequireAvc { get; set; }

        public bool RequireNonAnamorphic { get; set; }

        public bool CopyTimestamps { get; set; }

        public bool EnableMpegtsM2TsMode { get; set; }

        public bool EnableSubtitlesInManifest { get; set; }

        public string[] AudioCodecs { get; set; }

        public string[] VideoCodecs { get; set; }

        public int? AudioStreamIndex { get; set; }

        public int? SubtitleStreamIndex { get; set; }

        public int? TranscodingMaxAudioChannels { get; set; }

        public int? GlobalMaxAudioChannels { get; set; }

        public int? AudioBitrate { get; set; }

        public int? AudioSampleRate { get; set; }

        public int? VideoBitrate { get; set; }

        public int? MaxWidth { get; set; }

        public int? MaxHeight { get; set; }

        public float? MaxFramerate { get; set; }

        public DeviceProfile DeviceProfile { get; set; }

        public string DeviceProfileId { get; set; }

        public string DeviceId { get; set; }

        public long? RunTimeTicks { get; set; }

        public TranscodeSeekInfo TranscodeSeekInfo { get; set; }

        public bool EstimateContentLength { get; set; }

        public MediaSourceInfo MediaSource { get; set; }

        public string[] SubtitleCodecs { get; set; }

        public SubtitleDeliveryMethod SubtitleDeliveryMethod { get; set; }

        public string SubtitleFormat { get; set; }

        public string PlaySessionId { get; set; }

        public TranscodeReason[] TranscodeReasons { get; set; }

        public Dictionary<string, string> StreamOptions { get; private set; }

        public string MediaSourceId => MediaSource?.Id;

        public bool IsDirectStream =>
            PlayMethod == PlayMethod.DirectStream ||
            PlayMethod == PlayMethod.DirectPlay;

        /// <summary>
        /// Gets the audio stream that will be used.
        /// </summary>
        public MediaStream TargetAudioStream => MediaSource?.GetDefaultAudioStream(AudioStreamIndex);

        /// <summary>
        /// Gets the video stream that will be used.
        /// </summary>
        public MediaStream TargetVideoStream => MediaSource?.VideoStream;

        /// <summary>
        /// Gets the audio sample rate that will be in the output stream.
        /// </summary>
        public int? TargetAudioSampleRate
        {
            get
            {
                var stream = TargetAudioStream;
                return AudioSampleRate.HasValue && !IsDirectStream
                    ? AudioSampleRate
                    : stream == null ? null : stream.SampleRate;
            }
        }

        /// <summary>
        /// Gets the audio sample rate that will be in the output stream.
        /// </summary>
        public int? TargetAudioBitDepth
        {
            get
            {
                if (IsDirectStream)
                {
                    return TargetAudioStream == null ? (int?)null : TargetAudioStream.BitDepth;
                }

                var targetAudioCodecs = TargetAudioCodec;
                var audioCodec = targetAudioCodecs.Length == 0 ? null : targetAudioCodecs[0];
                if (!string.IsNullOrEmpty(audioCodec))
                {
                    return GetTargetAudioBitDepth(audioCodec);
                }

                return TargetAudioStream == null ? (int?)null : TargetAudioStream.BitDepth;
            }
        }

        /// <summary>
        /// Gets the audio sample rate that will be in the output stream.
        /// </summary>
        public int? TargetVideoBitDepth
        {
            get
            {
                if (IsDirectStream)
                {
                    return TargetVideoStream == null ? (int?)null : TargetVideoStream.BitDepth;
                }

                var targetVideoCodecs = TargetVideoCodec;
                var videoCodec = targetVideoCodecs.Length == 0 ? null : targetVideoCodecs[0];
                if (!string.IsNullOrEmpty(videoCodec))
                {
                    return GetTargetVideoBitDepth(videoCodec);
                }

                return TargetVideoStream == null ? (int?)null : TargetVideoStream.BitDepth;
            }
        }

        /// <summary>
        /// Gets the target reference frames.
        /// </summary>
        /// <value>The target reference frames.</value>
        public int? TargetRefFrames
        {
            get
            {
                if (IsDirectStream)
                {
                    return TargetVideoStream == null ? (int?)null : TargetVideoStream.RefFrames;
                }

                var targetVideoCodecs = TargetVideoCodec;
                var videoCodec = targetVideoCodecs.Length == 0 ? null : targetVideoCodecs[0];
                if (!string.IsNullOrEmpty(videoCodec))
                {
                    return GetTargetRefFrames(videoCodec);
                }

                return TargetVideoStream == null ? (int?)null : TargetVideoStream.RefFrames;
            }
        }

        /// <summary>
        /// Gets the audio sample rate that will be in the output stream.
        /// </summary>
        public float? TargetFramerate
        {
            get
            {
                var stream = TargetVideoStream;
                return MaxFramerate.HasValue && !IsDirectStream
                    ? MaxFramerate
                    : stream == null ? null : stream.AverageFrameRate ?? stream.RealFrameRate;
            }
        }

        /// <summary>
        /// Gets the audio sample rate that will be in the output stream.
        /// </summary>
        public double? TargetVideoLevel
        {
            get
            {
                if (IsDirectStream)
                {
                    return TargetVideoStream == null ? (double?)null : TargetVideoStream.Level;
                }

                var targetVideoCodecs = TargetVideoCodec;
                var videoCodec = targetVideoCodecs.Length == 0 ? null : targetVideoCodecs[0];
                if (!string.IsNullOrEmpty(videoCodec))
                {
                    return GetTargetVideoLevel(videoCodec);
                }

                return TargetVideoStream == null ? (double?)null : TargetVideoStream.Level;
            }
        }

        /// <summary>
        /// Gets the audio sample rate that will be in the output stream.
        /// </summary>
        public int? TargetPacketLength
        {
            get
            {
                var stream = TargetVideoStream;
                return !IsDirectStream
                    ? null
                    : stream == null ? null : stream.PacketLength;
            }
        }

        /// <summary>
        /// Gets the audio sample rate that will be in the output stream.
        /// </summary>
        public string TargetVideoProfile
        {
            get
            {
                if (IsDirectStream)
                {
                    return TargetVideoStream == null ? null : TargetVideoStream.Profile;
                }

                var targetVideoCodecs = TargetVideoCodec;
                var videoCodec = targetVideoCodecs.Length == 0 ? null : targetVideoCodecs[0];
                if (!string.IsNullOrEmpty(videoCodec))
                {
                    return GetOption(videoCodec, "profile");
                }

                return TargetVideoStream == null ? null : TargetVideoStream.Profile;
            }
        }

        /// <summary>
        /// Gets the target video codec tag.
        /// </summary>
        /// <value>The target video codec tag.</value>
        public string TargetVideoCodecTag
        {
            get
            {
                var stream = TargetVideoStream;
                return !IsDirectStream
                    ? null
                    : stream == null ? null : stream.CodecTag;
            }
        }

        /// <summary>
        /// Gets the audio bitrate that will be in the output stream.
        /// </summary>
        public int? TargetAudioBitrate
        {
            get
            {
                var stream = TargetAudioStream;
                return AudioBitrate.HasValue && !IsDirectStream
                    ? AudioBitrate
                    : stream == null ? null : stream.BitRate;
            }
        }

        /// <summary>
        /// Gets the audio channels that will be in the output stream.
        /// </summary>
        public int? TargetAudioChannels
        {
            get
            {
                if (IsDirectStream)
                {
                    return TargetAudioStream == null ? (int?)null : TargetAudioStream.Channels;
                }

                var targetAudioCodecs = TargetAudioCodec;
                var codec = targetAudioCodecs.Length == 0 ? null : targetAudioCodecs[0];
                if (!string.IsNullOrEmpty(codec))
                {
                    return GetTargetRefFrames(codec);
                }

                return TargetAudioStream == null ? (int?)null : TargetAudioStream.Channels;
            }
        }

        /// <summary>
        /// Gets the audio codec that will be in the output stream.
        /// </summary>
        public string[] TargetAudioCodec
        {
            get
            {
                var stream = TargetAudioStream;

                string inputCodec = stream?.Codec;

                if (IsDirectStream)
                {
                    return string.IsNullOrEmpty(inputCodec) ? Array.Empty<string>() : new[] { inputCodec };
                }

                foreach (string codec in AudioCodecs)
                {
                    if (string.Equals(codec, inputCodec, StringComparison.OrdinalIgnoreCase))
                    {
                        return string.IsNullOrEmpty(codec) ? Array.Empty<string>() : new[] { codec };
                    }
                }

                return AudioCodecs;
            }
        }

        public string[] TargetVideoCodec
        {
            get
            {
                var stream = TargetVideoStream;

                string inputCodec = stream?.Codec;

                if (IsDirectStream)
                {
                    return string.IsNullOrEmpty(inputCodec) ? Array.Empty<string>() : new[] { inputCodec };
                }

                foreach (string codec in VideoCodecs)
                {
                    if (string.Equals(codec, inputCodec, StringComparison.OrdinalIgnoreCase))
                    {
                        return string.IsNullOrEmpty(codec) ? Array.Empty<string>() : new[] { codec };
                    }
                }

                return VideoCodecs;
            }
        }

        /// <summary>
        /// Gets the audio channels that will be in the output stream.
        /// </summary>
        public long? TargetSize
        {
            get
            {
                if (IsDirectStream)
                {
                    return MediaSource.Size;
                }

                if (RunTimeTicks.HasValue)
                {
                    int? totalBitrate = TargetTotalBitrate;

                    double totalSeconds = RunTimeTicks.Value;
                    // Convert to ms
                    totalSeconds /= 10000;
                    // Convert to seconds
                    totalSeconds /= 1000;

                    return totalBitrate.HasValue ?
                        Convert.ToInt64(totalBitrate.Value * totalSeconds) :
                        (long?)null;
                }

                return null;
            }
        }

        public int? TargetVideoBitrate
        {
            get
            {
                var stream = TargetVideoStream;

                return VideoBitrate.HasValue && !IsDirectStream
                    ? VideoBitrate
                    : stream == null ? null : stream.BitRate;
            }
        }

        public TransportStreamTimestamp TargetTimestamp
        {
            get
            {
                var defaultValue = string.Equals(Container, "m2ts", StringComparison.OrdinalIgnoreCase)
                    ? TransportStreamTimestamp.Valid
                    : TransportStreamTimestamp.None;

                return !IsDirectStream
                    ? defaultValue
                    : MediaSource == null ? defaultValue : MediaSource.Timestamp ?? TransportStreamTimestamp.None;
            }
        }

        public int? TargetTotalBitrate => (TargetAudioBitrate ?? 0) + (TargetVideoBitrate ?? 0);

        public bool? IsTargetAnamorphic
        {
            get
            {
                if (IsDirectStream)
                {
                    return TargetVideoStream == null ? null : TargetVideoStream.IsAnamorphic;
                }

                return false;
            }
        }

        public bool? IsTargetInterlaced
        {
            get
            {
                if (IsDirectStream)
                {
                    return TargetVideoStream == null ? (bool?)null : TargetVideoStream.IsInterlaced;
                }

                var targetVideoCodecs = TargetVideoCodec;
                var videoCodec = targetVideoCodecs.Length == 0 ? null : targetVideoCodecs[0];
                if (!string.IsNullOrEmpty(videoCodec))
                {
                    if (string.Equals(GetOption(videoCodec, "deinterlace"), "true", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }

                return TargetVideoStream == null ? (bool?)null : TargetVideoStream.IsInterlaced;
            }
        }

        public bool? IsTargetAVC
        {
            get
            {
                if (IsDirectStream)
                {
                    return TargetVideoStream == null ? null : TargetVideoStream.IsAVC;
                }

                return true;
            }
        }

        public int? TargetWidth
        {
            get
            {
                var videoStream = TargetVideoStream;

                if (videoStream != null && videoStream.Width.HasValue && videoStream.Height.HasValue)
                {
                    ImageDimensions size = new ImageDimensions(videoStream.Width.Value, videoStream.Height.Value);

                    size = DrawingUtils.Resize(size, 0, 0, MaxWidth ?? 0, MaxHeight ?? 0);

                    return size.Width;
                }

                return MaxWidth;
            }
        }

        public int? TargetHeight
        {
            get
            {
                var videoStream = TargetVideoStream;

                if (videoStream != null && videoStream.Width.HasValue && videoStream.Height.HasValue)
                {
                    ImageDimensions size = new ImageDimensions(videoStream.Width.Value, videoStream.Height.Value);

                    size = DrawingUtils.Resize(size, 0, 0, MaxWidth ?? 0, MaxHeight ?? 0);

                    return size.Height;
                }

                return MaxHeight;
            }
        }

        public int? TargetVideoStreamCount
        {
            get
            {
                if (IsDirectStream)
                {
                    return GetMediaStreamCount(MediaStreamType.Video, int.MaxValue);
                }

                return GetMediaStreamCount(MediaStreamType.Video, 1);
            }
        }

        public int? TargetAudioStreamCount
        {
            get
            {
                if (IsDirectStream)
                {
                    return GetMediaStreamCount(MediaStreamType.Audio, int.MaxValue);
                }

                return GetMediaStreamCount(MediaStreamType.Audio, 1);
            }
        }

        public void SetOption(string qualifier, string name, string value)
        {
            if (string.IsNullOrEmpty(qualifier))
            {
                SetOption(name, value);
            }
            else
            {
                SetOption(qualifier + "-" + name, value);
            }
        }

        public void SetOption(string name, string value)
        {
            StreamOptions[name] = value;
        }

        public string GetOption(string qualifier, string name)
        {
            var value = GetOption(qualifier + "-" + name);

            if (string.IsNullOrEmpty(value))
            {
                value = GetOption(name);
            }

            return value;
        }

        public string GetOption(string name)
        {
            if (StreamOptions.TryGetValue(name, out var value))
            {
                return value;
            }

            return null;
        }

        public string ToUrl(string baseUrl, string accessToken)
        {
            if (PlayMethod == PlayMethod.DirectPlay)
            {
                return MediaSource.Path;
            }

            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }

            var sb = new StringBuilder();
            foreach (NameValuePair pair in BuildParams(this, accessToken))
            {
                sb.Append(pair.Name);
                sb.Append('=');
                sb.Append(pair.Value.Replace(" ", "%20"));
                sb.Append("%26");
            }

            string extension = string.IsNullOrEmpty(Container) ? string.Empty : "." + Container;

            baseUrl = baseUrl.TrimEnd('/');

            var mediaType = MediaType.ToString().ToLowerInvariant();
            if (string.Equals(SubProtocol, "hls", StringComparison.OrdinalIgnoreCase))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}/master.m3u8?{3}", baseUrl, mediaType, ItemId, sb.ToString());
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}/stream{3}?{4}", baseUrl, mediaType, ItemId, extension, sb.ToString());
        }

        private static List<NameValuePair> BuildParams(StreamInfo item, string accessToken)
        {
            var list = new List<NameValuePair>();

            // list.Add(new NameValuePair("dlnaheaders", "true"));

            if (!string.IsNullOrEmpty(item.DeviceProfileId))
            {
                list.Add(new NameValuePair("DeviceProfileId", item.DeviceProfileId));
            }

            if (!string.IsNullOrEmpty(item.DeviceId))
            {
                list.Add(new NameValuePair("DeviceId", item.DeviceId));
            }

            if (!string.IsNullOrEmpty(item.MediaSourceId))
            {
                list.Add(new NameValuePair("MediaSourceId", item.MediaSourceId));
            }

            // Be careful, IsDirectStream==true by default (Static != false or not in query).
            // See initialization of StreamingRequestDto in AudioController.GetAudioStream() method : Static = @static ?? true.
            var directStream = item.IsDirectStream.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
            if (!string.Equals(directStream, "true", StringComparison.OrdinalIgnoreCase))
            {
                list.Add(new NameValuePair("Static", directStream));
            }

            if (item.VideoCodecs.Length > 0)
            {
                list.Add(new NameValuePair("VideoCodec", string.Join(",", item.VideoCodecs)));
            }

            if (item.AudioCodecs.Length > 0)
            {
                list.Add(new NameValuePair("AudioCodec", string.Join(",", item.AudioCodecs)));
            }

            if (item.AudioStreamIndex != null)
            {
                list.Add(new NameValuePair("AudioStreamIndex", item.AudioStreamIndex.ToString()));
            }

            if (item.SubtitleDeliveryMethod != SubtitleDeliveryMethod.External)
            {
                if (item.SubtitleStreamIndex > -1)
                {
                    list.Add(new NameValuePair("SubtitleStreamIndex", item.SubtitleStreamIndex.ToString()));
                    list.Add(new NameValuePair("SubtitleMethod", item.SubtitleDeliveryMethod.ToString()));
                }
            }

            if (item.VideoBitrate != null)
            {
                list.Add(new NameValuePair("VideoBitrate", item.VideoBitrate.ToString()));
            }

            if (item.AudioBitrate != null)
            {
                list.Add(new NameValuePair("AudioBitrate", item.AudioBitrate.ToString()));
            }

            if (item.AudioSampleRate != null)
            {
                list.Add(new NameValuePair("AudioSampleRate", item.AudioSampleRate.ToString()));
            }

            if (item.MaxFramerate != null)
            {
                list.Add(new NameValuePair("MaxFramerate", item.MaxFramerate.ToString()));
            }

            if (item.MaxWidth != null)
            {
                list.Add(new NameValuePair("MaxWidth", item.MaxWidth.ToString()));
            }

            if (item.MaxHeight != null)
            {
                list.Add(new NameValuePair("MaxHeight", item.MaxHeight.ToString()));
            }

            long startPositionTicks = item.StartPositionTicks;

            var isHls = string.Equals(item.SubProtocol, "hls", StringComparison.OrdinalIgnoreCase);

            if (!isHls && startPositionTicks > 0)
            {
                list.Add(new NameValuePair("StartTimeTicks", startPositionTicks.ToString(CultureInfo.InvariantCulture)));
            }

            if (!string.IsNullOrEmpty(item.PlaySessionId))
            {
                list.Add(new NameValuePair("PlaySessionId", item.PlaySessionId));
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                list.Add(new NameValuePair("api_key", accessToken));
            }

            var liveStreamId = item.MediaSource?.LiveStreamId;
            if (!string.IsNullOrEmpty(liveStreamId))
            {
                list.Add(new NameValuePair("LiveStreamId", liveStreamId));
            }

            if (!item.IsDirectStream)
            {
                if (item.RequireNonAnamorphic)
                {
                    list.Add(new NameValuePair("RequireNonAnamorphic", item.RequireNonAnamorphic.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()));
                }

                if (item.TranscodingMaxAudioChannels != null)
                {
                    list.Add(new NameValuePair("TranscodingMaxAudioChannels", item.TranscodingMaxAudioChannels.ToString()));
                }

                if (item.EnableSubtitlesInManifest)
                {
                    list.Add(new NameValuePair("EnableSubtitlesInManifest", item.EnableSubtitlesInManifest.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()));
                }

                if (item.EnableMpegtsM2TsMode)
                {
                    list.Add(new NameValuePair("EnableMpegtsM2TsMode", item.EnableMpegtsM2TsMode.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()));
                }

                if (item.EstimateContentLength)
                {
                    list.Add(new NameValuePair("EstimateContentLength", item.EstimateContentLength.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()));
                }

                if (item.TranscodeSeekInfo != TranscodeSeekInfo.Auto)
                {
                    list.Add(new NameValuePair("TranscodeSeekInfo", item.TranscodeSeekInfo.ToString().ToLowerInvariant()));
                }

                if (item.CopyTimestamps)
                {
                    list.Add(new NameValuePair("CopyTimestamps", item.CopyTimestamps.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()));
                }

                list.Add(new NameValuePair("RequireAvc", item.RequireAvc.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()));
            }

            if (!string.IsNullOrEmpty(item.MediaSource.ETag))
            {
                list.Add(new NameValuePair("Tag", item.MediaSource.ETag));
            }

            if (item.SubtitleStreamIndex != null && item.SubtitleDeliveryMethod == SubtitleDeliveryMethod.Embed && item.SubtitleCodecs.Length > 0)
            {
                list.Add(new NameValuePair("SubtitleCodec", string.Join(",", item.SubtitleCodecs)));
            }

            if (isHls)
            {
                if (!string.IsNullOrEmpty(item.Container))
                {
                    list.Add(new NameValuePair("SegmentContainer", item.Container));
                }

                if (item.SegmentLength != null)
                {
                    list.Add(new NameValuePair("SegmentLength", item.SegmentLength.ToString()));
                }

                if (item.MinSegments != null)
                {
                    list.Add(new NameValuePair("MinSegments", item.MinSegments.ToString()));
                }

                list.Add(new NameValuePair("BreakOnNonKeyFrames", item.BreakOnNonKeyFrames.ToString(CultureInfo.InvariantCulture)));
            }

            foreach (var pair in item.StreamOptions)
            {
                if (string.IsNullOrEmpty(pair.Value))
                {
                    continue;
                }

                // strip spaces to avoid having to encode h264 profile names
                list.Add(new NameValuePair(pair.Key, pair.Value.Replace(" ", string.Empty)));
            }

            if (!item.IsDirectStream)
            {
                list.Add(new NameValuePair("TranscodeReasons", string.Join(',', item.TranscodeReasons.Distinct())));
            }

            return list;
        }

        public List<SubtitleStreamInfo> GetExternalSubtitles(ITranscoderSupport transcoderSupport, bool includeSelectedTrackOnly, string baseUrl, string accessToken)
        {
            return GetExternalSubtitles(transcoderSupport, includeSelectedTrackOnly, false, baseUrl, accessToken);
        }

        public List<SubtitleStreamInfo> GetExternalSubtitles(ITranscoderSupport transcoderSupport, bool includeSelectedTrackOnly, bool enableAllProfiles, string baseUrl, string accessToken)
        {
            var list = GetSubtitleProfiles(transcoderSupport, includeSelectedTrackOnly, enableAllProfiles, baseUrl, accessToken);
            var newList = new List<SubtitleStreamInfo>();

            // First add the selected track
            foreach (SubtitleStreamInfo stream in list)
            {
                if (stream.DeliveryMethod == SubtitleDeliveryMethod.External)
                {
                    newList.Add(stream);
                }
            }

            return newList;
        }

        public List<SubtitleStreamInfo> GetSubtitleProfiles(ITranscoderSupport transcoderSupport, bool includeSelectedTrackOnly, string baseUrl, string accessToken)
        {
            return GetSubtitleProfiles(transcoderSupport, includeSelectedTrackOnly, false, baseUrl, accessToken);
        }

        public List<SubtitleStreamInfo> GetSubtitleProfiles(ITranscoderSupport transcoderSupport, bool includeSelectedTrackOnly, bool enableAllProfiles, string baseUrl, string accessToken)
        {
            var list = new List<SubtitleStreamInfo>();

            // HLS will preserve timestamps so we can just grab the full subtitle stream
            long startPositionTicks = string.Equals(SubProtocol, "hls", StringComparison.OrdinalIgnoreCase)
                ? 0
                : (PlayMethod == PlayMethod.Transcode && !CopyTimestamps ? StartPositionTicks : 0);

            // First add the selected track
            if (SubtitleStreamIndex.HasValue)
            {
                foreach (var stream in MediaSource.MediaStreams)
                {
                    if (stream.Type == MediaStreamType.Subtitle && stream.Index == SubtitleStreamIndex.Value)
                    {
                        AddSubtitleProfiles(list, stream, transcoderSupport, enableAllProfiles, baseUrl, accessToken, startPositionTicks);
                    }
                }
            }

            if (!includeSelectedTrackOnly)
            {
                foreach (var stream in MediaSource.MediaStreams)
                {
                    if (stream.Type == MediaStreamType.Subtitle && (!SubtitleStreamIndex.HasValue || stream.Index != SubtitleStreamIndex.Value))
                    {
                        AddSubtitleProfiles(list, stream, transcoderSupport, enableAllProfiles, baseUrl, accessToken, startPositionTicks);
                    }
                }
            }

            return list;
        }

        private void AddSubtitleProfiles(List<SubtitleStreamInfo> list, MediaStream stream, ITranscoderSupport transcoderSupport, bool enableAllProfiles, string baseUrl, string accessToken, long startPositionTicks)
        {
            if (enableAllProfiles)
            {
                foreach (var profile in DeviceProfile.SubtitleProfiles)
                {
                    var info = GetSubtitleStreamInfo(stream, baseUrl, accessToken, startPositionTicks, new[] { profile }, transcoderSupport);

                    list.Add(info);
                }
            }
            else
            {
                var info = GetSubtitleStreamInfo(stream, baseUrl, accessToken, startPositionTicks, DeviceProfile.SubtitleProfiles, transcoderSupport);

                list.Add(info);
            }
        }

        private SubtitleStreamInfo GetSubtitleStreamInfo(MediaStream stream, string baseUrl, string accessToken, long startPositionTicks, SubtitleProfile[] subtitleProfiles, ITranscoderSupport transcoderSupport)
        {
            var subtitleProfile = StreamBuilder.GetSubtitleProfile(MediaSource, stream, subtitleProfiles, PlayMethod, transcoderSupport, Container, SubProtocol);
            var info = new SubtitleStreamInfo
            {
                IsForced = stream.IsForced,
                Language = stream.Language,
                Name = stream.Language ?? "Unknown",
                Format = subtitleProfile.Format,
                Index = stream.Index,
                DeliveryMethod = subtitleProfile.Method,
                DisplayTitle = stream.DisplayTitle
            };

            if (info.DeliveryMethod == SubtitleDeliveryMethod.External)
            {
                if (MediaSource.Protocol == MediaProtocol.File || !string.Equals(stream.Codec, subtitleProfile.Format, StringComparison.OrdinalIgnoreCase) || !stream.IsExternal)
                {
                    info.Url = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}/Videos/{1}/{2}/Subtitles/{3}/{4}/Stream.{5}",
                        baseUrl,
                        ItemId,
                        MediaSourceId,
                        stream.Index.ToString(CultureInfo.InvariantCulture),
                        startPositionTicks.ToString(CultureInfo.InvariantCulture),
                        subtitleProfile.Format);

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        info.Url += "?api_key=" + accessToken;
                    }

                    info.IsExternalUrl = false;
                }
                else
                {
                    info.Url = stream.Path;
                    info.IsExternalUrl = true;
                }
            }

            return info;
        }

        public int? GetTargetVideoBitDepth(string codec)
        {
            var value = GetOption(codec, "videobitdepth");
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }

        public int? GetTargetAudioBitDepth(string codec)
        {
            var value = GetOption(codec, "audiobitdepth");
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }

        public double? GetTargetVideoLevel(string codec)
        {
            var value = GetOption(codec, "level");
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }

        public int? GetTargetRefFrames(string codec)
        {
            var value = GetOption(codec, "maxrefframes");
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }

        public int? GetTargetAudioChannels(string codec)
        {
            var defaultValue = GlobalMaxAudioChannels ?? TranscodingMaxAudioChannels;

            var value = GetOption(codec, "audiochannels");
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            {
                return Math.Min(result, defaultValue ?? result);
            }

            return defaultValue;
        }

        private int? GetMediaStreamCount(MediaStreamType type, int limit)
        {
            var count = MediaSource.GetStreamCount(type);

            if (count.HasValue)
            {
                count = Math.Min(count.Value, limit);
            }

            return count;
        }

        public List<MediaStream> GetSelectableAudioStreams()
        {
            return GetSelectableStreams(MediaStreamType.Audio);
        }

        public List<MediaStream> GetSelectableSubtitleStreams()
        {
            return GetSelectableStreams(MediaStreamType.Subtitle);
        }

        public List<MediaStream> GetSelectableStreams(MediaStreamType type)
        {
            var list = new List<MediaStream>();

            foreach (var stream in MediaSource.MediaStreams)
            {
                if (type == stream.Type)
                {
                    list.Add(stream);
                }
            }

            return list;
        }
    }
}
