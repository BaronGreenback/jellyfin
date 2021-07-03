using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Session;

namespace Jellyfin.Model.Tests.Dlna
{
    public class LegacyStreamInfo : StreamInfo
    {
        public LegacyStreamInfo(Guid itemId, DlnaProfileType mediaType, DeviceProfile profile)
        : base(itemId, mediaType, profile)
        {
        }

        /// <summary>
        /// The 10.6 ToUrl code from StreamInfo.cs with which to compare new version.
        /// </summary>
        /// <param name="baseUrl">The base url to use.</param>
        /// <param name="accessToken">The Access token.</param>
        /// <returns>A url.</returns>
        public string ToUrl_Original(string baseUrl, string accessToken)
        {
            if (PlayMethod == PlayMethod.DirectPlay)
            {
                return MediaSource?.Path ?? string.Empty;
            }

            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }

            var list = new List<string>();
            foreach (NameValuePair pair in BuildParams(this, accessToken))
            {
                if (string.IsNullOrEmpty(pair.Value))
                {
                    continue;
                }

                // Try to keep the url clean by omitting defaults
                if (string.Equals(pair.Name, "StartTimeTicks", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(pair.Value, "0", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.Equals(pair.Name, "SubtitleStreamIndex", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(pair.Value, "-1", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.Equals(pair.Name, "Static", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(pair.Value, "false", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var encodedValue = pair.Value.Replace(" ", "%20", StringComparison.Ordinal);

                list.Add(string.Format(CultureInfo.InvariantCulture, "{0}={1}", pair.Name, encodedValue));
            }

            string queryString = string.Join("&", list.ToArray());

            return GetUrl(baseUrl, queryString);
        }

        private string GetUrl(string baseUrl, string queryString)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }

            string extension = string.IsNullOrEmpty(Container) ? string.Empty : "." + Container;

            baseUrl = baseUrl.TrimEnd('/');

            if (MediaType == DlnaProfileType.Audio)
            {
                if (string.Equals(SubProtocol, "hls", StringComparison.OrdinalIgnoreCase))
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}/audio/{1}/master.m3u8?{2}", baseUrl, ItemId, queryString);
                }

                return string.Format(CultureInfo.InvariantCulture, "{0}/audio/{1}/stream{2}?{3}", baseUrl, ItemId, extension, queryString);
            }

            if (string.Equals(SubProtocol, "hls", StringComparison.OrdinalIgnoreCase))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}/videos/{1}/master.m3u8?{2}", baseUrl, ItemId, queryString);
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}/videos/{1}/stream{2}?{3}", baseUrl, ItemId, extension, queryString);
        }

        private static List<NameValuePair> BuildParams(StreamInfo item, string accessToken)
        {
            var list = new List<NameValuePair>();

            string audioCodecs = item.AudioCodecs.Length == 0 ?
                string.Empty :
                string.Join(",", item.AudioCodecs);

            string videoCodecs = item.VideoCodecs.Length == 0 ?
                string.Empty :
                string.Join(",", item.VideoCodecs);

            list.Add(new NameValuePair("DeviceProfileId", item.DeviceProfileId ?? string.Empty));
            list.Add(new NameValuePair("DeviceId", item.DeviceId ?? string.Empty));
            list.Add(new NameValuePair("MediaSourceId", item.MediaSourceId ?? string.Empty));
            list.Add(new NameValuePair("Static", item.IsDirectStream.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()));
            list.Add(new NameValuePair("VideoCodec", videoCodecs));
            list.Add(new NameValuePair("AudioCodec", audioCodecs));
            list.Add(new NameValuePair("AudioStreamIndex", item.AudioStreamIndex.HasValue ? item.AudioStreamIndex.Value.ToString(CultureInfo.InvariantCulture) : string.Empty));
            list.Add(new NameValuePair("SubtitleStreamIndex", item.SubtitleStreamIndex.HasValue && item.SubtitleDeliveryMethod != SubtitleDeliveryMethod.External ? item.SubtitleStreamIndex.Value.ToString(CultureInfo.InvariantCulture) : string.Empty));
            list.Add(new NameValuePair("VideoBitrate", item.VideoBitrate.HasValue ? item.VideoBitrate.Value.ToString(CultureInfo.InvariantCulture) : string.Empty));
            list.Add(new NameValuePair("AudioBitrate", item.AudioBitrate.HasValue ? item.AudioBitrate.Value.ToString(CultureInfo.InvariantCulture) : string.Empty));
            list.Add(new NameValuePair("AudioSampleRate", item.AudioSampleRate.HasValue ? item.AudioSampleRate.Value.ToString(CultureInfo.InvariantCulture) : string.Empty));

            list.Add(new NameValuePair("MaxFramerate", item.MaxFramerate.HasValue ? item.MaxFramerate.Value.ToString(CultureInfo.InvariantCulture) : string.Empty));
            list.Add(new NameValuePair("MaxWidth", item.MaxWidth.HasValue ? item.MaxWidth.Value.ToString(CultureInfo.InvariantCulture) : string.Empty));
            list.Add(new NameValuePair("MaxHeight", item.MaxHeight.HasValue ? item.MaxHeight.Value.ToString(CultureInfo.InvariantCulture) : string.Empty));

            long startPositionTicks = item.StartPositionTicks;

            var isHls = string.Equals(item.SubProtocol, "hls", StringComparison.OrdinalIgnoreCase);

            if (isHls)
            {
                list.Add(new NameValuePair("StartTimeTicks", string.Empty));
            }
            else
            {
                list.Add(new NameValuePair("StartTimeTicks", startPositionTicks.ToString(CultureInfo.InvariantCulture)));
            }

            list.Add(new NameValuePair("PlaySessionId", item.PlaySessionId ?? string.Empty));
            list.Add(new NameValuePair("api_key", accessToken ?? string.Empty));

            var liveStreamId = item.MediaSource?.LiveStreamId;
            list.Add(new NameValuePair("LiveStreamId", liveStreamId ?? string.Empty));

            list.Add(new NameValuePair("SubtitleMethod", item.SubtitleStreamIndex.HasValue && item.SubtitleDeliveryMethod != SubtitleDeliveryMethod.External ? item.SubtitleDeliveryMethod.ToString() : string.Empty));

            if (!item.IsDirectStream)
            {
                if (item.RequireNonAnamorphic)
                {
                    list.Add(new NameValuePair("RequireNonAnamorphic", item.RequireNonAnamorphic.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()));
                }

                list.Add(new NameValuePair("TranscodingMaxAudioChannels", item.TranscodingMaxAudioChannels.HasValue ? item.TranscodingMaxAudioChannels.Value.ToString(CultureInfo.InvariantCulture) : string.Empty));

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

            list.Add(new NameValuePair("Tag", item.MediaSource?.ETag ?? string.Empty));

            string subtitleCodecs = item.SubtitleCodecs.Length == 0 ?
               string.Empty :
               string.Join(",", item.SubtitleCodecs);

            list.Add(new NameValuePair("SubtitleCodec", item.SubtitleStreamIndex.HasValue && item.SubtitleDeliveryMethod == SubtitleDeliveryMethod.Embed ? subtitleCodecs : string.Empty));

            if (isHls)
            {
                list.Add(new NameValuePair("SegmentContainer", item.Container ?? string.Empty));

                if (item.SegmentLength.HasValue)
                {
                    list.Add(new NameValuePair("SegmentLength", item.SegmentLength.Value.ToString(CultureInfo.InvariantCulture)));
                }

                if (item.MinSegments.HasValue)
                {
                    list.Add(new NameValuePair("MinSegments", item.MinSegments.Value.ToString(CultureInfo.InvariantCulture)));
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
                list.Add(new NameValuePair(pair.Key, pair.Value.Replace(" ", string.Empty, StringComparison.Ordinal)));
            }

            if (!item.IsDirectStream)
            {
                list.Add(new NameValuePair("TranscodeReasons", string.Join(',', item.TranscodeReasons.Distinct())));
            }

            return list;
        }
    }
}