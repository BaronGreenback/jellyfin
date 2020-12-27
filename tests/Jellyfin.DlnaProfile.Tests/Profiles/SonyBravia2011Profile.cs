#pragma warning disable SA1118 // Parameter should not span multiple lines

using MediaBrowser.Model.Dlna;

namespace Jellyfin.DlnaProfiles.Profiles
{
    /// <summary>
    /// Defines the <see cref="SonyBravia2011Profile" />.
    /// </summary>
    [System.Xml.Serialization.XmlRoot("Profile")]
    public class SonyBravia2011Profile : DefaultProfile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SonyBravia2011Profile"/> class.
        /// </summary>
        public SonyBravia2011Profile()
        {
            Name = "Sony Bravia (2011)";

            Identification = new DeviceIdentification(
                @"KDL-\d{2}([A-Z]X\d2\d|CX400).*",
                new[]
                {
                    new HttpHeaderInfo
                    {
                        Name = "X-AV-Client-Info",
                        Value = @".*KDL-\d{2}([A-Z]X\d2\d|CX400).*",
                        Match = HeaderMatchType.Regex
                    }
                })
            {
                Manufacturer = "Sony"
            };

            AddXmlRootAttribute("xmlns:av", "urn:schemas-sony-com:av");

            AlbumArtPn = "JPEG_TN";

            ModelName = "Windows Media Player Sharing";
            ModelNumber = "3.0";
            ModelUrl = "http://www.microsoft.com/";
            Manufacturer = "Microsoft Corporation";
            ManufacturerUrl = "http://www.microsoft.com/";
            SonyAggregationFlags = "10";
            EnableSingleAlbumArtLimit = true;
            EnableAlbumArtInDidl = true;

            TranscodingProfiles = new[]
            {
                new TranscodingProfile("mp3", "mp3"),
                new TranscodingProfile("ts", "h264", "ac3")
                {
                    EnableMpegtsM2TsMode = true
                },
                new TranscodingProfile("jpeg")
            };

            DirectPlayProfiles = new[]
            {
                new DirectPlayProfile("ts,mpegts", "h264", "ac3,aac,mp3"),
                new DirectPlayProfile("ts,mpegts", "mpeg2video", "mp3"),
                new DirectPlayProfile("mp4,m4v", "h264,mpeg4", "ac3,aac,mp3"),
                new DirectPlayProfile("mpeg", "mpeg2video,mpeg1video", "mp3"),
                new DirectPlayProfile("asf", "wmv2,wmv3,vc1", "wmav2,wmapro,wmavoice"),
                new DirectPlayProfile("mp3", "mp3"),
                new DirectPlayProfile("asf", "wmav2,wmapro,wmavoice")
            };

            ContainerProfiles = new[]
            {
                new ContainerProfile(
                    DlnaProfileType.Photo,
                    new[]
                    {
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.Width, "1920"),
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.Height, "1080")
                    })
            };

            ResponseProfiles = new[]
            {
                new ResponseProfile("ts,mpegts", DlnaProfileType.Video, "video/vnd.dlna.mpeg-tts")
                {
                    VideoCodec = "h264",
                    AudioCodec = "ac3,aac,mp3",
                    OrgPn = "AVC_TS_HD_24_AC3_T,AVC_TS_HD_50_AC3_T,AVC_TS_HD_60_AC3_T,AVC_TS_HD_EU_T",
                    Conditions = new[]
                    {
                        new ProfileCondition(ProfileConditionType.Equals, ProfileConditionValue.PacketLength, "192"),
                        new ProfileCondition(ProfileConditionType.Equals, ProfileConditionValue.VideoTimestamp, "Valid")
                    }
                },

                new ResponseProfile("ts,mpegts", DlnaProfileType.Video, "video/mpeg")
                {
                    VideoCodec = "h264",
                    AudioCodec = "ac3,aac,mp3",
                    OrgPn = "AVC_TS_HD_24_AC3_ISO,AVC_TS_HD_50_AC3_ISO,AVC_TS_HD_60_AC3_ISO,AVC_TS_HD_EU_ISO",
                    Conditions = new[]
                    {
                        new ProfileCondition(ProfileConditionType.Equals, ProfileConditionValue.PacketLength, "188")
                    }
                },

                new ResponseProfile("ts,mpegts", DlnaProfileType.Video, "video/vnd.dlna.mpeg-tts")
                {
                    VideoCodec = "h264",
                    AudioCodec = "ac3,aac,mp3",
                    OrgPn = "AVC_TS_HD_24_AC3,AVC_TS_HD_50_AC3,AVC_TS_HD_60_AC3,AVC_TS_HD_EU",
                },

                new ResponseProfile("ts,mpegts", DlnaProfileType.Video, "video/vnd.dlna.mpeg-tts")
                {
                    VideoCodec = "mpeg2video",
                    OrgPn = "MPEG_TS_SD_EU,MPEG_TS_SD_NA,MPEG_TS_SD_KO",
                },

                new ResponseProfile("mpeg", DlnaProfileType.Video, "video/mpeg")
                {
                    VideoCodec = "mpeg1video,mpeg2video",
                    OrgPn = "MPEG_PS_NTSC,MPEG_PS_PAL",
                },

                new ResponseProfile("m4v", DlnaProfileType.Video, "video/mp4")
            };

            CodecProfiles = new[]
            {
                new CodecProfile(
                    "h264",
                    CodecType.Video,
                    new[]
                    {
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.Width, "1920"),
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.Height, "1080"),
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.VideoFramerate, "30"),
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.VideoBitrate, "20000000"),
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.VideoLevel, "41")
                    }),

                new CodecProfile(
                    "mpeg2video",
                    CodecType.Video,
                    new[]
                    {
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.Width, "1920"),
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.Height, "1080"),
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.VideoFramerate, "30"),
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.VideoBitrate, "20000000")
                    }),

                new CodecProfile(
                    null,
                    CodecType.Video,
                    new[]
                    {
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.Width, "1920"),
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.Height, "1080"),
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.VideoFramerate, "30")
                    }),

                new CodecProfile(
                    "ac3",
                    CodecType.VideoAudio,
                    new[]
                    {
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.AudioChannels, "6")
                    }),

                new CodecProfile(
                    "aac",
                    CodecType.VideoAudio,
                    new[]
                    {
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.AudioChannels, "2"),
                        new ProfileCondition(ProfileConditionType.NotEquals, ProfileConditionValue.AudioProfile, "he-aac")
                    }),

                new CodecProfile(
                    "mp3,mp2",
                    CodecType.VideoAudio,
                    new[]
                    {
                        new ProfileCondition(ProfileConditionType.LessThanEqual, ProfileConditionValue.AudioChannels, "2")
                    })
            };

            SubtitleProfiles = new[]
            {
                new SubtitleProfile("srt", SubtitleDeliveryMethod.Embed)
            };
        }
    }
}
