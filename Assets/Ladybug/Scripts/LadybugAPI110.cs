//=============================================================================
// Copyright © 2008 Point Grey Research, Inc. All Rights Reserved.
//
// This software is the confidential and proprietary information of Point
// Grey Research, Inc. ("Confidential Information").  You shall not
// disclose such Confidential Information and shall use it only in
// accordance with the terms of the license agreement you entered into
// with Point Grey Research, Inc. (PGR).
//
// PGR MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE SUITABILITY OF THE
// SOFTWARE, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE, OR NON-INFRINGEMENT. PGR SHALL NOT BE LIABLE FOR ANY DAMAGES
// SUFFERED BY LICENSEE AS A RESULT OF USING, MODIFYING OR DISTRIBUTING
// THIS SOFTWARE OR ITS DERIVATIVES.
//=============================================================================
//
// This file defines the interface that C# programs need to use Ladybug SDK.
// This file must be added to your C# project.
//
//=============================================================================

using System;
using System.Runtime.InteropServices;
using System.Text;


namespace LadybugAPI110
{
    // Description:
    //   An enumeration of all possible errors returned by the Ladybug API.
    public enum LadybugError
    {
        /** Function completed successfully. */
        LADYBUG_OK,

        /** General failure. */
        LADYBUG_FAILED,

        /** Invalid argument passed to the function. */
        LADYBUG_INVALID_ARGUMENT,

        /** Invalid context passed to the function. */
        LADYBUG_INVALID_CONTEXT,

        /** The functionality is not implemented with this version of the library. */
        LADYBUG_NOT_IMPLEMENTED,

        /**
         * The functionality is not supported with the current software or hardware
         * configurations.
         */
        LADYBUG_NOT_SUPPORTED,

        /** The device or context has already been initialized. */
        LADYBUG_ALREADY_INITIALIZED,

        /** Grabbing has already been started. */
        LADYBUG_ALREADY_STARTED,

        /** Failed to open file. */
        LADYBUG_COULD_NOT_OPEN_FILE,

        /** Memory allocation error. */
        LADYBUG_MEMORY_ALLOC_ERROR,

        /** There is not enough space on the disk. */
        LADYBUG_ERROR_DISK_NOT_ENOUGH_SPACE,

        /** Stream file has not opened. */
        LADYBUG_STREAM_FILE_NOT_OPENED,

        /** Invalid stream file name. */
        LADYBUG_INVALID_STREAM_FILE_NAME,

        /** Device or object not initialized. */
        LADYBUG_NOT_INITIALIZED,

        /** Camera has not been started. */
        LADYBUG_NOT_STARTED,

        /** Request would exceed maximum bandwidth of the bus. */
        LADYBUG_MAX_BANDWIDTH_EXCEEDED,

        /** Invalid video mode or frame rate passed or retrieved. */
        LADYBUG_INVALID_VIDEO_SETTING,

        /** The rectify resolution has not been properly set. */
        LADYBUG_NEED_RECTIFY_RESOLUTION,

        /** Function is deprecated - please see documentation. */
        LADYBUG_DEPRECATED,

        /**
         * The image buffer returned by the camera was too small to contain all of
         * the JPEG image data.
         */
        LADYBUG_IMAGE_TOO_SMALL,

        /** Operation timed out. */
        LADYBUG_TIMEOUT,

        /** Too many image buffers are locked by the user. */
        LADYBUG_TOO_MANY_LOCKED_BUFFERS,

        /** No calibration file was found on the Ladybug head unit. */
        LADYBUG_CALIBRATION_FILE_NOT_FOUND,

        /** An error occurred during JPEG decompression. */
        LADYBUG_JPEG_ERROR,

        /** An error occurred in JPEG header verification. */
        LADYBUG_JPEG_HEADER_ERROR,

        /** The compressor did not have enough time to finish compressing the data. */
        LADYBUG_JPEG_INCOMPLETE_COMPRESSION,

        /** There is no image in this frame. */
        LADYBUG_JPEG_NO_IMAGE,

        /** The compressor detected a corrupted image. */
        LADYBUG_CORRUPTED_IMAGE_DATA,

        /** An error occurred in off-screen buffer initialization. */
        LADYBUG_OFFSCREEN_BUFFER_INIT_ERROR,

        /** Unsupported framebuffer format. */
        LADYBUG_FRAMEBUFFER_UNSUPPORTED_FORMAT,

        /** Framebuffer incomplete. */
        LADYBUG_FRAMEBUFFER_INCOMPLETE,

        /** GPS device could not be started. */
        LADYBUG_GPS_COULD_NOT_BE_STARTED,

        /** GPS has not been started. */
        LADYBUG_GPS_NOT_STARTED,

        /** No GPS data. */
        LADYBUG_GPS_NO_DATA,

        /** No GPS data for this sentence. */
        LADYBUG_GPS_NO_DATA_FOR_THIS_SENTENCE,

        /** GPS communication port may be in use. */
        LADYBUG_GPS_COMM_PORT_IN_USE,

        /** GPS communication port does not exist. */
        LADYBUG_GPS_COMM_PORT_DOES_NOT_EXIST,

        /** OpenGL display list has not initialized. */
        LADYBUG_OPENGL_DISPLAYLIST_NOT_INITIALIZED,

        /** OpenGL image texture has not updated. */
        LADYBUG_OPENGL_TEXTUREIMAGE_NOT_UPDATED,

        /** OpenGL device context is invalid. */
        LADYBUG_INVALID_OPENGL_DEVICE_CONTEXT,

        /** OpenGL rendering context is invalid. */
        LADYBUG_INVALID_OPENGL_RENDERING_CONTEXT,

        /** OpenGL texture is invalid. */
        LADYBUG_INVALID_OPENGL_TEXTURE,

        /** The requested OpenGL operation is not valid. */
        LADYBUG_INVALID_OPENGL_OPERATION,

        /** There are not enough resources available for image texture. */
        LADYBUG_NOT_ENOUGH_RESOURCE_FOR_OPENGL_TEXTURE,

        /**
         * The current rendering context failed to share the display-list space
         * of another rendering context
         */
        LADYBUG_SHARING_DISPLAYLIST_FAILED,

        /** The specified off-screen image is invalid. */
        LADYBUG_INVALID_OFFSCREEN_BUFFER_SIZE,

        /** The requested job is still on-going. */
        LADYBUG_STILL_WORKING,

        /** The PGR stream is corrupted and cannot be corrected. */
        LADYBUG_CORRUPTED_PGR_STREAM,

        /** There is no device supporting CUDA, or driver and runtime version may be mismatched. */
        LADYBUG_GPU_CUDA_DRIVER_ERROR,

        /** An error occurred in GPU functions. */
        LADYBUG_GPU_ERROR,

        /** Low level failure */
        LADYBUG_LOW_LEVEL_FAILURE,

        /** Register failure */
        LADYBUG_REGISTER_FAILED,

        /** Isoch-related failure */
        LADYBUG_ISOCH_FAILED,

        /** Buffer retrieval failure */
        LADYBUG_RETRIEVE_BUFFER_FAILED,

        /** Image library failure */
        LADYBUG_IMAGE_LIBRARY_FAILED,

        /** Busmaster-related failure */
        LADYBUG_BUS_MASTER_FAILED,

        /** Unknown error. */
        LADYBUG_ERROR_UNKNOWN,

        /** Voltage error */
        LADYBUG_BAD_VOLTAGE,

        /** Interface error (eg. USB2 instead of USB3 on LD5) */
        LADYBUG_BAD_INTERFACE,

        /** Number of errors */
        LADYBUG_NUM_LADYBUG_ERRORS,

        /** Unused member. */
        LADYBUG_ERROR_FORCE_QUADLET = 0x7FFFFFFF,
    };

    //
    // Description:
    //   An enumeration of the different camera properties for the Ladybug.
    //
    // Remarks:
    //   Many of these properties are included only for completeness and future
    //   expandability, and will have no effect on a Ladybug camera.
    //
    public enum LadybugProperty
    {
        LADYBUG_BRIGHTNESS, /**< The brightness property. */
        LADYBUG_AUTO_EXPOSURE, /**< The auto exposure property. */
        LADYBUG_SHARPNESS, /**< The sharpness property. Not supported */
        LADYBUG_WHITE_BALANCE, /**< The white balance property. */
        LADYBUG_HUE, /**< The hue property. Not supported */
        LADYBUG_SATURATION, /**< The saturation property. Not supported */
        LADYBUG_GAMMA, /**< The gamma property. */
        LADYBUG_IRIS, /**< The iris property. Not supported */
        LADYBUG_FOCUS, /**< The focus property. Not supported */
        LADYBUG_ZOOM, /**< The zoom property. Not supported */
        LADYBUG_PAN, /**< The pan property. Not supported */
        LADYBUG_TILT, /**< The tilt property. Not supported */
        LADYBUG_SHUTTER, /**< The shutter property. */
        LADYBUG_GAIN, /*<* The gain property. */
        LADYBUG_FRAME_RATE, /**< The camera heads frame rate */
        LADYBUG_PROPERTY_FORCE_QUADLET = 0x7FFFFFFF, /**< Unused member. */
    };

    public enum LadybugIndependentProperty
    {
        LADYBUG_SUB_GAIN, /**< Per-camera gain settings. */   
        LADYBUG_SUB_SHUTTER, /**< Per-camera shutter settings. */  
        LADYBUG_SUB_AUTO_EXPOSURE, /**< Per-camera auto exposure settings as well as "cameras of interest". */   
        LADYBUG_SUB_FORCE_QUADLET = 0x7FFFFFFF, /**< Unused member. */
    };

    //
    // Description:
    //   Error codes returned from all PGRCameraGUI functions.
    //
    public enum CameraGUIError
    {
        // Function completed successfully.
        PGRCAMGUI_OK,
        // Function failed.
        PGRCAMGUI_FAILED,
        // Was unable to create dialog.
        PGRCAMGUI_COULD_NOT_CREATE_DIALOG,
        // An invalid argument was passed.
        PGRCAMGUI_INVALID_ARGUMENT,
        // An invalid context was passed.
        PGRCAMGUI_INVALID_CONTEXT,
        // Memory allocation error.
        PGRCAMGUI_MEMORY_ALLOCATION_ERROR,
        // There has been an internal camera error - call getLastError()
        PGRCAMGUI_INTERNAL_CAMERA_ERROR,

    };

    public enum LadybugDataFormat
    {
        /**
         * This format produces a single image buffer that has each sensor's image
         * one after the other. Again, each pixel is in its raw 8bpp format.
         */
        LADYBUG_DATAFORMAT_RAW8 = 1,

        /**
         * This format is similar to LADYBUG_DATAFORMAT_RAW8 except that the entire
         * buffer is JPEG compressed.  This format is intended for use with cameras
         * that have black and white sensors.
         */
        LADYBUG_DATAFORMAT_JPEG8,

        /**
         * This format separates each individual image into its 4 individual Bayer
         * channels (Green, Red, Blue and Green - not necessarily in that order).
         */
        LADYBUG_DATAFORMAT_COLOR_SEP_RAW8,

        /**
         * Similar to LADYBUG_DATAFORMAT_COLOR_SEP_RAW8 except that the
         * transmitted buffer is JPEG compressed.
         */
        LADYBUG_DATAFORMAT_COLOR_SEP_JPEG8,

        /**
         * Similar to LADYBUG_DATAFORMAT_RAW8.
         * The height of the image is only half of that in LADYBUG_DATAFORMAT_RAW8 format.
         */
        LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW8,

        /**
         * Similar to LADYBUG_DATAFORMAT_COLOR_SEP_JPEG8.
         * The height of the image is only half of the original image.
         * This format is only supported by Ladybug3.
         */
        LADYBUG_DATAFORMAT_COLOR_SEP_HALF_HEIGHT_JPEG8,

        /**
         * This format produces a single image buffer that has each sensor's image
         * one after the other. Each pixel is in raw 16bpp format.
         */
        LADYBUG_DATAFORMAT_RAW16,

        /**
         * Similar to LADYBUG_DATAFORMAT_COLOR_SEP_JPEG8 except that the image
         * data is 12bit JPEG compressed.
         */
        LADYBUG_DATAFORMAT_COLOR_SEP_JPEG12,

        /**
         * Similar to LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW8.
         * Each pixel is in raw 16bpp format.
         */
        LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW16,

        /**
         * Similar to LADYBUG_DATAFORMAT_COLOR_SEP_HALF_HEIGHT_JPEG8 except that
         * the image data is 12bit JPEG compressed.
         */
        LADYBUG_DATAFORMAT_COLOR_SEP_HALF_HEIGHT_JPEG12,

        /**
         * This format produces a single image buffer that has each sensor's image
         * one after the other. Each pixel is in raw 12bpp format.
         *
         * The image data is laid out as follows (24 bytes / 2 pixels):
         * Px1 (top 8 bytes) | Px2 (top 8 bytes) | Px1 (bottom 4 bytes) | Px2 (bottom 4 bytes)
         */
        LADYBUG_DATAFORMAT_RAW12,

        /**
         * Similar to LADYBUG_DATAFORMAT_RAW12.
         * The height of the image is only half of that in LADYBUG_DATAFORMAT_RAW12 format.
         */
        LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW12,

        /** The number of possible data formats. */
        LADYBUG_NUM_DATAFORMATS,

        /** Hook for "any usable video mode". */
        LADYBUG_DATAFORMAT_ANY,

        /** Unused member. */
        LADYBUG_DATAFORMAT_FORCE_QUADLET = 0x7FFFFFFF,
    };


    public enum LadybugSaveFileFormat
    {
        LADYBUG_FILEFORMAT_PGM, /**< 8-bit greyscale .PGM */
        LADYBUG_FILEFORMAT_PPM, /**< 24 bit .PPM */
        LADYBUG_FILEFORMAT_BMP, /**< 24 bit .BMP */
        LADYBUG_FILEFORMAT_JPG, /**< JPEG image */
        LADYBUG_FILEFORMAT_PNG, /**< PNG image */
        LADYBUG_FILEFORMAT_TIFF, /**< TIFF image */
        LADYBUG_FILEFORMAT_EXIF, /**< EXIF image, GPS information is stored in EXIF tags present in LadybugProcessedImage. */
        LADYBUG_FILEFORMAT_HDR, /**< HDR (Radiance) format */
        LADYBUG_FILEFORMAT_FORCE_QUADLET = 0x7FFFFFFF, /**< Unused member. */
    };



    public enum LadybugResolution
    {
        LADYBUG_RESOLUTION_1024x768 = 4, /**< 1024x768 pixels. Ladybug2 camera. */
        LADYBUG_RESOLUTION_1616x1232 = 8, /**< 1616x1232 pixels. Ladybug3 camera.  */
        LADYBUG_RESOLUTION_2448x2048 = 9, /**< 2448x2048 pixels. Ladybug5 camera.  */
        LADYBUG_NUM_RESOLUTIONS = 10, /**< Number of possible resolutions. */
        LADYBUG_RESOLUTION_ANY = 11, /**< Hook for any usable resolution. */
        LADYBUG_RESOLUTION_FORCE_QUADLET = 0x7FFFFFFF, /**< Unused member. */
    };

    public enum LadybugStippledFormat
    {
        LADYBUG_BGGR, /**< BGGR image. */
        LADYBUG_GBRG, /**< GBRG image. */
        LADYBUG_GRBG, /**< GRBG image. */
        LADYBUG_RGGB, /**< RGGB image. */
        LADYBUG_DEFAULT, /**< Default stippled format for the camera. */
        LADYBUG_STIPPLED_FORCE_QUADLET = 0x7FFFFFFF, /**< Unused member. */
    };

    public enum LadybugPixelFormat
    {
        LADYBUG_MONO8 = 0x00000001, /**< 8 bit mono */
        LADYBUG_MONO16 = 0x00000020, /**< 16 bit mono */
        LADYBUG_RAW8 = 0x00000200, /**< 8 bit raw data */
        LADYBUG_RAW16 = 0x00000400, /**< 16 bit raw data */
        LADYBUG_BGR = 0x10000001, /**< 24 bit BGR */
        LADYBUG_BGRU = 0x10000002, /**< 32 bit BGRU */
        LADYBUG_BGR16 = 0x10000004, /**< 48 bit BGR (16 bit int per channel) */
        LADYBUG_BGRU16 = 0x10000008, /**< 64 bit BGRU (16 bit int per channel) */
        LADYBUG_BGR16F = 0x10000010, /**< 48 bit BGR (16 bit float per channel) */
        LADYBUG_BGR32F = 0x10000020, /**< 96 bit BGR (32 bit float per channel) */
        LADYBUG_PIXELFORMAT_FORCE_QUADLET = 0x7FFFFFFF, /**< Unused member. */
        LADYBUG_UNSPECIFIED_PIXEL_FORMAT = 0, /**< Unspecified pixel format. */
    };

    public enum LadybugColorProcessingMethod
    {
        /**
        * Disable color processing - This is useful for retrieving the
        * original bayer patten image. When the image is the color-separated
        * JPEG stream, the JPEG data is decompressed and the 4 color-separated
        * channels are combined into one bayer image.
        */
        LADYBUG_DISABLE,

        /**
         * Edge sensing de-mosaicing - This is the most accurate method
         * that can keep up with the camera's frame rate.
         */
        LADYBUG_EDGE_SENSING,

        /**
         * Nearest neighbour de-mosaicing (fast) - Faster, less accurate than
         * nearest neighbor de-mosaicing.
         */
        LADYBUG_NEAREST_NEIGHBOR_FAST,

        /**
         *  Rigorous de-mosaicing - This provides the second best quality colour
         *  reproduction.  This method is very processor intensive and may
         *  not keep up with the camera's frame rate.  Best used for
         *  offline processing where accurate colour reproduction is required.
         */
        LADYBUG_RIGOROUS,

        /**
         * Downsample4 mode - Color process to output a half width and half height 
         * image. This allows for faster previews and processing. This results in
         * an output image that is 1/4 the size of the source image.
         */
        LADYBUG_DOWNSAMPLE4,

        /**
         * Downsample16 mode - Color process to output a quarter width and quarter  
         * height image. This allows for faster previews and processing. This
         * results in an image that is 1/16th the size of the source image.
         */
        LADYBUG_DOWNSAMPLE16,

        /**
         * Mono - This processing method only uses the green color channel to
         * generate grey scale Ladybug images. It is designed for fast previews of
         * compressed JPEG image streams. This method also downsamples the image
         * as in LADYBUG_DOWNSAMPLE4 so the result image is quarter size.
         */
        LADYBUG_MONO,

        /**
         * High quality linear interpolation - This algorithm provides similar
         * results to Rigorous, but is up to 30 times faster.
         */
        LADYBUG_HQLINEAR,

        /**
         * High quality linear interpolation - This algorithm is the same with
         * LADYBUG_HQLINEAR, but the color processing is performed on GPU.
         */
        LADYBUG_HQLINEAR_GPU,

        /**
         * A de-mosaicking algorithm based on the directional filter - This should
         * give the best image quality.
         */
        LADYBUG_DIRECTIONAL_FILTER,

        /** Unused member. */
        LADYBUG_COLOR_FORCE_QUADLET = 0x7FFFFFFF,
    };

    public enum LadybugToneMappingMode
    {    
        LADYBUG_TONE_MAPPING_NONE,
        LADYBUG_TONE_MAPPING_OPENGL,
        LADYBUG_TONE_MAPPING_CPU,


        /** Unused member. */
        LADYBUG_TONE_MAPPING_FORCE_QUADLET = 0x7FFFFFFF 
    };

    unsafe public struct LadybugImageInfo
    {
        // Constant fingerprint, should be LADYBUGIMAGEINFO_STRUCT_FINGERPRINT.
        public uint ulFingerprint;
        // Structure version number, should be 0x00000002.
        public uint ulVersion;
        // Timestamp, in seconds, since the UNIX time epoch.
        // If it is 0, all the data in LadybugImageInfo are invalid.
        public uint ulTimeSeconds;
        // Microsecond fraction of above second.
        public uint ulTimeMicroSeconds;
        // Sequence number of the image.  Reset to zero when the head powers up
        //  and incremented for every image.
        public uint ulSequenceId;
        // Horizontal refresh rate. (reserved)
        public uint ulHRate;
        // Actual adjusted gains used by each of the 6 cameras.  Similar to the
        //  DCAM gain register.
        public fixed uint arulGainAdjust[6];
        // A copy of the DCAM whitebalance register.
        public uint ulWhiteBalance;
        // This is the same as register 0x1044, described in the PGR IEEE 1394
        //  Register Reference.
        public uint ulBayerGain;
        // This is the same as register 0x1040, described in the PGR IEEE 1394
        //  Register Reference.
        public uint ulBayerMap;
        // A copy of the Brightness DCAM register.
        public uint ulBrightness;
        // A copy of the Gamma DCAM register.
        public uint ulGamma;
        // The serial number of the Ladybug head.
        public uint ulSerialNum;
        // Shutter values for each sensor.  Similar to the DCAM shutter register.
        public fixed uint ulShutter[6];
        // GPS Latitude, < 0 = South of Equator, > 0 = North of Equator.
        // If dGPSLatitude = LADYBUG_INVALID_GPS_DATA(defined in ladybugstream.h),
        // the data is invalid
        public double dGPSLatitude;
        // GPS Longitude, < 0 = West of Prime Meridian, > 0 = East of Prime Meridian.
        // If dGPSLongitude = LADYBUG_INVALID_GPS_DATA(defined in ladybugstream.h),
        // the data is invalid
        public double dGPSLongitude;
        // GPS Antenna Altitude above/below mean-sea-level (geoid) (in meters).
        // If dGPSAltitude = LADYBUG_INVALID_GPS_DATA(defined in ladybugstream.h),
        // the data is invalid
        public double dGPSAltitude;

    };

    /**
     * In certain data formats, the camera may transmit the fully-masked pixels
     * at the edges of the sensor in addition to the useful image data.
     *
     * When available, this information is used to adjust the black level of
     * the image data during ladybugConvertImage().
     */
    unsafe public struct LadybugImageBorder
    {
        public uint uiTopRows; /**< Number of pixels of border on top of image */
        public uint uiBottomRows; /**< Number of pixels of border on bottom of image */
        public uint uiLeftCols; /**< Number of pixels of border on left of image */
        public uint uiRightCols; /**< Number of pixels of border on right of image */
    };

    /** Image information for this image */
    unsafe public struct LadybugImageHeader
    {
        public uint uiTemperature;
        public uint uiHumidity;
        public uint uiAirPressure;
        LadybugTriplet compass;
        LadybugTriplet accelerometer;
        LadybugTriplet gyroscope;
        bool needSoftwareAdjustment;
    };

    /** Simple structure to hold information from a 3-axis sensor. */
    unsafe public struct LadybugTriplet
    {
        float x;
        float y;
        float z;
    };

    public enum LadybugOutputImage
    {
        // Decompressed and color processed images
        LADYBUG_RAW_CAM0 = (0x1 << 0),
        LADYBUG_RAW_CAM1 = (0x1 << 1),
        LADYBUG_RAW_CAM2 = (0x1 << 2),
        LADYBUG_RAW_CAM3 = (0x1 << 3),
        LADYBUG_RAW_CAM4 = (0x1 << 4),
        LADYBUG_RAW_CAM5 = (0x1 << 5),
        LADYBUG_ALL_RAW_IMAGES = 0x0000003F,

        // Rectified images
        LADYBUG_RECTIFIED_CAM0 = (0x1 << 6),
        LADYBUG_RECTIFIED_CAM1 = (0x1 << 7),
        LADYBUG_RECTIFIED_CAM2 = (0x1 << 8),
        LADYBUG_RECTIFIED_CAM3 = (0x1 << 9),
        LADYBUG_RECTIFIED_CAM4 = (0x1 << 10),
        LADYBUG_RECTIFIED_CAM5 = (0x1 << 11),
        LADYBUG_ALL_RECTIFIED_IMAGES = 0x00000FC0,

        // Panoramic image
        LADYBUG_PANORAMIC = (0x1 << 12),

        // Dome projection image
        LADYBUG_DOME = (0x1 << 13),

        // Spherical image
        LADYBUG_SPHERICAL = (0x1 << 14),

        // All decompressed and color processed images in one view
        LADYBUG_ALL_CAMERAS_VIEW = (0x1 << 15),

        // All output images
        LADYBUG_ALL_OUTPUT_IMAGE = 0x7FFFFFFF,

    };

    public struct LadybugTimestamp
    {
        // The number of seconds since the epoch.
        public uint ulSeconds;
        // The microseconds component.
        public uint ulMicroSeconds;
        // The cycle time seconds.  0-127.
        public uint ulCycleSeconds;
        // The cycle time count.  0-7999 (1/8000ths of a second.)
        public uint ulCycleCount;
        // The cycle offset.  0-3071 (1/3072ths of a cycle count.)
        public uint ulCycleOffset;
    };

    unsafe public struct LadybugImage
    {
        // Columns, in pixels, of a single sensor image.
        public uint uiCols;

        // Rows, in pixels, of a single sensor image.
        public uint uiRows;

        /** Dimensions of the image border. */
        public LadybugImageBorder imageBorder;

        /**
        * Columns, in pixels, of the full sensor image. This is equal to
        * uiCols + imageBorder.uiLeft + imageBorder.uiRight.
        */
        public uint uiFullCols;

        /**
        * Rows, in pixels, of the full sensor image. This is equal to
        * uiRows + imageBorder.uiTop + imageBorder.uiBottom.
        */
        public uint uiFullRows;

        // The data format of associated image buffer contained in pData.
        public LadybugDataFormat dataFormat;

        // The per-sensor resolution of the returned image.
        public LadybugResolution resolution;

        // Timestamp of this image.
        public LadybugTimestamp timeStamp;

        // Image information for this image.
        public LadybugImageInfo imageInfo;

        /** Header **/
        LadybugImageHeader imageHeader;

        // Pointer to the image data.  The format is defined by dataFormat.
        public byte* pData;

        // Indicates whether the raw image data is stippled or not.
        public bool bStippled;

        /** Bayer pattern of image data. */
        LadybugStippledFormat stippledFormat;

        // Real data size, in bytes, of the data pointed to by pData.  Useful for
        // non-constant sizes (JPEG images).
        public uint uiDataSizeBytes;

        // The internal buffer index that the image buffer corresponds to.
        // For functions that lock the image, this number must be passed back to
        // the "unlock" function.  If ladybugInitializePlus() was called, this
        // number corresponds to the position of the buffer in the buffer array
        // passed in.
        // If the image is from a .pgr stream file, uiBufferIndex is not used.
        public uint uiBufferIndex;

        // Reserved for future image information
        public fixed uint ulReserved[3];

    };

    unsafe public struct LadybugStreamHeadInfo
    {
        public uint ulLadybugStreamVersion;
        public uint ulFrameRate;
        public uint serialBase;
        public uint serialHead;
        public fixed uint reserved[25];
        public uint ulPaddingSize;

        public LadybugDataFormat dataFormat;
        public LadybugResolution resolution;
        public LadybugStippledFormat stippledFormat;

        public uint ulConfigrationDataSize;
        public uint ulNumberOfImages;
        public uint ulNumberOfKeyIndex;
        public uint ulIncrement;
        public uint ulStreamDataOffset;
        public uint ulGPSDataOffset;
        public uint ulGPSDataSize;

        public uint ulFrameHeaderSize;

        public bool isHumidityAvailable;
        public float humidityMin;
        public float humidityMax;

        public bool isAirPressureAvailable;
        public float airPressureMin;
        public float airPressureMax;


        public bool isCompassAvailable;
        public float compassMin;
        public float compassMax;

        public bool isAccelerometerAvailable;
        public float accelerometerMin;
        public float accelerometerMax;


        public bool isGyroscopeAvailable;
        public float gyroscopeMin;
        public float gyroscopeMax;

        public float frameRate;

        public fixed uint reservedSpace[212];
        public fixed uint ulOffsetTable[512];
    };

    unsafe public struct LadybugImageMetaData
    {
        // GPS data

        // Hour, in Coordinated Universal Time
        public byte ucGPSHour;
        // Minute, in Coordinated Universal Time
        public byte ucGPSMinute;
        // Second, in Coordinated Universal Time
        public byte ucGPSSecond;
        // Hundredth of a second
        public ushort wGPSSubSecond;
        // Day
        public byte ucGPSDay;
        // Month
        public byte ucGPSMonth;
        // Year
        public ushort wGPSYear;
        // Latitude, <0 south of Equator, >0 north of Equator
        public double dGPSLatitude;
        // Longitude, <0 west of Prime Meridian, >0 east of Prime Meridian
        public double dGPSLongitude;
        // Antenna altitude.
        public double dGPSAltitude;
        // Ground speed, in kilometers per hour
        public double dGPSGroundSpeed;

        // Reserved.
        public fixed uint ulReserved[50];

    };

    unsafe public struct LadybugProcessedImage
    {
        // Columns, in pixels, of the stitched image
        public uint uiCols;

        // Rows, in pixels, of the stitched image
        public uint uiRows;

        // Pointer to the image data
        public byte* pData;

        // The pixel format of this image
        public LadybugPixelFormat pixelFormat;

        // Metadata of the image.
        public LadybugImageMetaData metaData;

        // Reserved
        public uint ulReserved;
    };

    unsafe public struct LadybugStabilizationParams
    {
        public int iNumTemplates;
        public int iMaximumSearchRegion;
        public double dDecayRate;
        public fixed int reserved[28];
    };

    unsafe public struct LadybugDynamicStitchingParams
    {
        // The range of distances being searched by the dynamic stitching mechanism.
        // If the distances to the subjects in the scene falls within a known range, it
        // can be set to avoid false matching.
        // The values are in meters and the default values for dMinimumDistance is 2,
        // dMaximumDistance is 100, and dDefaultDistance is 20.
        //
        public double dMinimumDistance;
        public double dMaximumDistance;
        public double dDefaultDistance;

        public fixed int reserved[26];
    };

    unsafe public struct LadybugToneMappingParams
    {
        /** This value determines how much compression is applied to the image.
          * This value has to be larger than 0.
          */
        public double dCompressionScale;

        /**
         * This value determines the size of the local area when calculating the
         * average intensity of the local area for a given pixel.
         * This value has to be larger or equal to 0. If it is 0, the local average
         * is determined by the pixel itself, so it works as a global compression
         * operator.
         */
        public double dLocalAreaSize;

        public LadybugToneMappingMode toneMappingMode;

        public fixed int reserved[27];

    };

    unsafe public struct LadybugColorCorrectionParams
    {
        // The hue value.
        public int iHue;

        // The saturation value.
        public int iSaturation;

        // The intensity value.
        public int iIntensity;

        // The red value
        public int iRed;

        // The green value
        public int iGreen;

        // The blue value
        public int iBlue;

        // Reserved field. Should not be used.
        public fixed int reserved[25];
    };

    unsafe public struct LadybugPoint3d
    {
        // Spherical Coordinate X.
        public float fX;

        // Spherical Coordinate Y.
        public float fY;

        // Spherical Coordinate Z.
        public float fZ;

        // Radial Coordinate Theta. Ranges from -PI (right) to +PI (left).
        public float fTheta;

        // Radial Coordinate Phi. Ranges from zero (up) to PI (down).
        public float fPhi;

        // Cylindrical Coordinate Angle, Ranges from -PI (right) to +PI (left).
        public float fCylAngle;

        // Cylindrical Coordinate Height, actual projected height on the cylinder.
        public float fCylHeight;
    };

    public enum LadybugBusSpeed
    {
        LADYBUG_S100,
        LADYBUG_S200,
        LADYBUG_S400,
        LADYBUG_S800,
        LADYBUG_S1600,
        LADYBUG_S3200,
        LADYBUG_S_FASTEST,
        LADYBUG_SPEED_UNKNOWN = -1,
        LADYBUG_SPEED_FORCE_QUADLET = 0x7FFFFFFF,
    };

    public enum LadybugInterfaceType
    {
        LADYBUG_INTERFACE_IEEE1394,
        LADYBUG_INTERFACE_USB2,
        LADYBUG_INTERFACE_USB3,
        LADYBUG_INTERFACE_UNKNOWN,
        LADYBUG_INTERFACE_FORCE_QUADLET = 0x7FFFFFFF,
    };

    public enum LadybugDeviceType
    {
        LADYBUG_DEVICE_LADYBUG,
        LADYBUG_DEVICE_COMPRESSOR,
        LADYBUG_DEVICE_LADYBUG3,
        LADYBUG_DEVICE_LADYBUG5,
        LADYBUG_DEVICE_UNKNOWN,
        LADYBUG_DEVICE_FORCE_QUADLET = 0x7FFFFFFF,
    };

    public enum LadybugAutoShutterRange
    {
        LADYBUG_AUTO_SHUTTER_MOTION,
        LADYBUG_AUTO_SHUTTER_INDOOR,
        LADYBUG_AUTO_SHUTTER_LOW_NOISE,
        LADYBUG_AUTO_SHUTTER_CUSTOM,
        LADYBUG_AUTO_SHUTTER_FORCE_QUADLET = 0x7FFFFFFF
    };

    public enum LadybugAutoExposureRoi
    {
        LADYBUG_AUTO_EXPOSURE_ROI_FULL_IMAGE, 
        LADYBUG_AUTO_EXPOSURE_ROI_BOTTOM_50,
        LADYBUG_AUTO_EXPOSURE_ROI_TOP_50, 
        LADYBUG_AUTO_EXPOSURE_ROI_FORCE_QUADLET = 0x7FFFFFFF 
    };

    public enum LadybugGainAdjustmentType
    {
        GAIN_DISABLE,
        GAIN_MANUAL,
        GAIN_FIX_EXPOSURE,
        GAIN_AUTOMATIC_COMPENSATION,
	    GAIN_AUTOMATIC_COMPENSATION_INDEPENDENT
    };

    public enum LadybugAdjustmentType
    {
        DISABLE,
        AUTOMATIC,
        MANUAL
    };

    public enum LadybugSmearCorrectionType
    {
        SMEAR_DISABLE,
        SMEAR_REMOVE,
        SMEAR_REMOVE_FILL, 
    };

    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct LadybugAdjustmentParameters
    {
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
        public bool doAdjustment;

        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
        public bool blackLevelAdjustmentType;

        public float blackLevel;

        public LadybugGainAdjustmentType gainAdjustmentType;

        public LadybugAutoExposureRoi gainRoi;

        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
        public bool considerGammaInGainAdjustment;

        public float gainManualValue;

        public float exposureTarget;

        public float exposureCompensation;

        public LadybugAdjustmentType whiteBalanceAdjustmentType;

        public float gainRed_ManualValue;

        public float gainBlue_ManualValue;

        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
        public bool gammaAdjustmentType;

        public float gammaManualValue;

        public LadybugSmearCorrectionType smearAlgo;

        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
        public bool noiseReduction;
    };

    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct LadybugCameraInfo
    {
        public uint serialBase;
        public uint serialHead;

        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
        public bool bIsColourCamera;

        public LadybugDeviceType deviceType;

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string pszModelName;

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string pszSensorInfo;

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string pszVendorName;

        public int iDCAMVer;
        public int iBusNum;
        public int iNodeNum;

        public LadybugBusSpeed maxBusSpeed;

        public LadybugInterfaceType interfaceType;
    };

    unsafe public struct LadybugH264Option
    {
        /** Frame rate of the stream */
        public float frameRate;

        /** Width of source image */
        public uint width;

        /** Height of source image */
        public uint height;

        /** Bitrate to encode at */
        public uint bitrate;

        /** Reserved for future use */
        public fixed uint reserved[256];
    };

    // This class defines static functions to access most of the
    // Ladybug APIs defined in ladybug.h, ladybuggeom.h, ladybugrenderer.h
    // and ladybugstream.h.
    unsafe public partial class Ladybug
    {
		//        public const string LADYBUG_DLL = "ladybug.dll";
		public const string LADYBUG_DLL = "Ladybug\\1.10\\ladybug";
      	//public const string LADYBUG_DLL = "Ladybug\\1.10\\FlyCap2CameraControl";
		
		public const int LADYBUG_NUM_CAMERAS = 6;

        //
        // ladybug.h functions
        //
        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugCreateContext")]
        public static extern LadybugError CreateContext(out int context);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugDestroyContext")]
        public static extern LadybugError DestroyContext(ref int context);

        // Need to use IntPtr not string since the string should not be freed
        // See:  MSDN "Memory Management with the Interop Marshaler"
        // Note: You can use System.Runtime.InteropServices.Marshal.PtrToStringAnsi()
        //       to get a managed string.
        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugErrorToString")]
        public static extern IntPtr ErrorToString(LadybugError errorCode);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetColorTileFormat")]
        public static extern LadybugError SetColorTileFormat(
          int context,
          LadybugStippledFormat format);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugConvertImage")]
        public static extern LadybugError ConvertImage(
                    int context,
                    ref LadybugImage pImage,
                    byte** arpDestBuffers,
                    LadybugPixelFormat pixelFormat);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugConvertImageBuffersPixelFormat")]
        public static extern LadybugError ConvertImageBuffersPixelFormat(
                    int context,
                    byte** arpBuffersIn,
                    byte** arpBuffersOut,
                    uint numBuffers,
                    uint numCols,
                    uint numRows,
                    LadybugPixelFormat pixelFormatIn,
                    LadybugPixelFormat pixelFormatOut);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetColorCorrectionFlag")]
        public static extern LadybugError GetColorCorrectionFlag(
                             int context,
                             out bool flag);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetColorCorrectionFlag")]
        public static extern LadybugError SetColorCorrectionFlag(
                             int context,
                             bool flag);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetColorCorrection")]
        public static extern LadybugError GetColorCorrection(int context,
                           out LadybugColorCorrectionParams ccparams);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetColorCorrection")]
        public static extern LadybugError SetColorCorrection(int context,
                           ref LadybugColorCorrectionParams ccparams);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetSharpening")]
        public static extern LadybugError GetSharpening(
                             int context,
                             out bool flag);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetSharpening")]
        public static extern LadybugError SetSharpening(
                             int context,
                             bool flag);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetCameraInfo")]
        public static extern LadybugError GetCameraInfo(int context, ref LadybugCameraInfo info);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetColorProcessingMethod")]
        public static extern LadybugError SetColorProcessingMethod(
            int context,
            LadybugColorProcessingMethod method);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetColorProcessingMethod")]
        public static extern LadybugError GetColorProcessingMethod(
            int context,
            out LadybugColorProcessingMethod method);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugStart")]
        public static extern LadybugError Start(
            int context,
            LadybugDataFormat format);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugStartEx")]
        public static extern LadybugError StartEx(
            int context,
            LadybugDataFormat format,
            uint packetSize,
            uint bufferSize);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugStartLockNext")]
        public static extern LadybugError StartLockNext(
            int context,
            LadybugDataFormat format);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugStartLockNextEx")]
        public static extern LadybugError StartLockNextEx(
            int context,
            LadybugDataFormat format,
            uint packetSize,
            uint bufferSize);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGrabImage")]
        public static extern LadybugError GrabImage(
                       int context,
                       out LadybugImage pImage);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugLockNext")]
        public static extern LadybugError LockNext(
                       int context,
                       out LadybugImage pImage);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugUnlock")]
        public static extern LadybugError Unlock(
                       int context,
                       uint bufferIndex);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugUnlockAll")]
        public static extern LadybugError UnlockAll(int context);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugBusEnumerateCameras")]
        public static extern LadybugError BusEnumerateCameras(
                       int context,
                       [In, Out] LadybugCameraInfo[] camInfos,
                       ref uint size);        

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugInitializeFromIndex")]
        public static extern LadybugError InitializeFromIndex(int context, uint ulDevice);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugInitializeFromSerialNumber")]
        public static extern LadybugError InitializeFromSerialNumber(int context, int serialNumber);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugStop")]
        public static extern LadybugError Stop(int context);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetProperty")]
        public static extern LadybugError SetProperty(int context,
                             LadybugProperty property,
                             int valueA,
                             int valueB,
                             bool auto);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetPropertyEx")]
        public static extern LadybugError SetPropertyEx(int context,
                             LadybugProperty property,
                             bool onePush,
                             bool onOff,
                             bool auto,
                             int valueA,
                             int valueB);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetAbsProperty")]
        public static extern LadybugError SetAbsProperty(int context,
                             LadybugProperty property,
                             float valueA);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetAbsPropertyEx")]
        public static extern LadybugError SetAbsPropertyEx(int context,
                             LadybugProperty property,
                             bool onePush,
                             bool onOff,
                             bool auto,
                             float value);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetProperty")]
        public static extern LadybugError GetProperty(int context,
                             LadybugProperty property,
                             out int valueA,
                             out int valueB,
                             out bool auto);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetPropertyEx")]
        public static extern LadybugError GetPropertyEx(int context,
                             LadybugProperty property,
                             out bool onePush,
                             out bool onOff,
                             out bool auto,
                             out int valueA,
                             out int valueB);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetAbsProperty")]
        public static extern LadybugError GetAbsProperty(int context,
                             LadybugProperty property,
                             out float valueA);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetAbsPropertyEx")]
        public static extern LadybugError GetAbsPropertyEx(int context,
                             LadybugProperty property,
                             out bool onePush,
                             out bool onOff,
                             out bool auto,
                             out float value);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetPropertyRange")]
        public static extern LadybugError GetPropertyRange(int context,
                             LadybugProperty property,
                             out int min,
                             out int max,
                             out int _default,
                             out bool auto,
                             out bool manual);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetPropertyRangeEx")]
        public static extern LadybugError GetPropertyRangeEx(int context,
                             LadybugProperty property,
                             out bool present,
                             out bool onePush,
                             out bool readOut,
                             out bool onOff,
                             out bool auto,
                             out bool manual,
                             out int min,
                             out int max);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetAbsPropertyRange")]
        public static extern LadybugError GetAbsPropertyRange(int context,
                             LadybugProperty property,
                             out bool present,
                             out float min,
                             out float max,
                             out IntPtr units,
                             out IntPtr unitAbbr);

        
        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetIndProperty")]
        public static extern LadybugError GetIndProperty(
                            int context,
                            LadybugIndependentProperty property,
                            uint uiCamera,
                            out ulong pulValue,
                            out bool pbOnOff,
                            out bool pbAuto,
                            out uint puiAutoExpCams );

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetIndProperty")]
        public static extern LadybugError SetIndProperty(
                            int context,
                            LadybugIndependentProperty property,
                            uint uiCamera,
                            ulong ulValue,
                            bool bOnOff,
                            bool bAuto,
                            uint uiAutoExpCams );

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetAutoShutterRange")]
        public static extern LadybugError GetAutoShutterRange(
            int context,
            out LadybugAutoShutterRange autoShutterRange);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetAutoShutterRange")]
        public static extern LadybugError SetAutoShutterRange(
            int context,
            LadybugAutoShutterRange autoShutterRange);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetAutoExposureROI")]
        public static extern LadybugError GetAutoExposureROI(
            int context,
            out LadybugAutoExposureRoi roi);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetAutoExposureROI")]
        public static extern LadybugError SetAutoExposureROI(
            int context,
            LadybugAutoExposureRoi roi);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetRegister")]
        public static extern LadybugError GetRegister(int context,
                             uint register,
                             out uint value);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetRegister")]
        public static extern LadybugError SetRegister(int context,
                             uint register,
                             uint value);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetJPEGQuality")]
        public static extern LadybugError SetJPEGQuality(int context,
                             int quality);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetJPEGQuality")]
        public static extern LadybugError GetJPEGQuality(int context,
                             out int quality);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetAutoJPEGQualityControlFlag")]
        public static extern LadybugError SetAutoJPEGQualityControlFlag(int context,
                             bool autoJPEGQualityControl);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetAutoJPEGQualityControlFlag")]
        public static extern LadybugError GetAutoJPEGQualityControlFlag(int context,
                             out bool autoJPEGQualityControl);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetAutoJPEGBufferUsage")]
        public static extern LadybugError GetAutoJPEGBufferUsage(int context, out uint puiBufferUsage);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetAutoJPEGBufferUsage")]
        public static extern LadybugError SetAutoJPEGBufferUsage(int context, uint uiBufferUsage);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetFalloffCorrectionFlag")]
        public static extern LadybugError SetFalloffCorrectionFlag(int context,
                           bool correctFalloff);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetFalloffCorrectionAttenuation")]
        public static extern LadybugError SetFalloffCorrectionAttenuation(int context,
                          float attenuationFraction);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugEnableImageStabilization")]
        public static extern LadybugError EnableImageStabilization(int context,
                           bool enable,
                           ref LadybugStabilizationParams stabilizationParams);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSaveImage")]
        public static extern LadybugError SaveImage(int context,
                           ref LadybugProcessedImage processedImage,
                           string path,
                           LadybugSaveFileFormat format,
                           bool async);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugDoOneShotAutoWhiteBalance")]
        public static extern LadybugError DoOneShotAutoWhiteBalance(int context);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetOneShotAutoWhiteBalanceStatus")]
        public static extern LadybugError GetOneShotAutoWhiteBalanceStatus(int context);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetToneMappingFlag")]
        public static extern LadybugError ladybugSetToneMappingFlag(int context, bool enable);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetToneMappingFlag")]
        public static extern LadybugError ladybugGetToneMappingFlag(int context, out bool enable);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetToneMappingParams")]
        public static extern LadybugError SetToneMappingParams(int context, ref LadybugToneMappingParams toneMappingParams);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetToneMappingParams")]
        public static extern LadybugError GetToneMappingParams(int context, out LadybugToneMappingParams toneMappingParams);

        //
        // ladybuggeom.h functions
        //
        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugInitializeAlphaMasks")]
        public static extern LadybugError InitializeAlphaMasks(
                int context,
                uint uiCols,
                uint uiRows,
                bool writeToFile);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugLoadAlphaMasks")]
        public static extern LadybugError LoadAlphaMasks(
                int context,
                uint uiCols,
                uint uiRows);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugLoadConfig")]
        public static extern LadybugError LoadConfig(int context, string path);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetAlphaMasking")]
        public static extern LadybugError SetAlphaMasking(int context, bool enable);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSet3dMapRotation")]
        public static extern LadybugError Set3dMapRotation(int context, double dRx, double dRy, double dRz);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGet3dMapRotation")]
        public static extern LadybugError Get3dMapRotation(int context, out double dRx, out double dRy, out double dRz);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSet3dMapSphereSize")]
        public static extern LadybugError Set3dMapSphereSize(int context, double dRadius);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGet3dMapSphereSize")]
        public static extern LadybugError Get3dMapSphereSize(int context, out double dRadius);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetDynamicStitching")]
        public static extern LadybugError SetDynamicStitching(int context, bool onePush, bool auto, ref LadybugPoint3d point);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetDynamicStitching")]
        public static extern LadybugError GetDynamicStitching(int context, out bool auto);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetDynamicStitchingParams")]
        public static extern LadybugError SetDynamicStitchingParams(int context, ref LadybugDynamicStitchingParams dsparams);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetDynamicStitchingParams")]
        public static extern LadybugError GetDynamicStitchingParams(int context, out LadybugDynamicStitchingParams dsparams);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetBlendingParams")]
        public static extern LadybugError SetBlendingParams(int context, double dMaxBlendingWidth);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetBlendingParams")]
        public static extern LadybugError GetBlendingParams(int context, out double dMaxBlendingWidth);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetCameraUnitFocalLength")]
        public static extern LadybugError GetCameraUnitFocalLength(int context, int camera, out double focalLength);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetCameraUnitImageCenter")]
        public static extern LadybugError GetCameraUnitImageCenter(int context, int camera, out double centerX, out double centerY);

        //
        // ladybugrenderer.h functions
        //
        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugConfigureOutputImages")]
        public static extern LadybugError ConfigureOutputImages(
                int context,
                uint uiImageTypes);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetOffScreenImageSize")]
        public static extern LadybugError SetOffScreenImageSize(
                int context,
                LadybugOutputImage imageType,
                uint uiCols,
                uint uiRows);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugRenderOffScreenImage")]
        public static extern LadybugError RenderOffScreenImage(
             int context,
             LadybugOutputImage imageType,
             LadybugPixelFormat pixelFormat,
             out LadybugProcessedImage pImage);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetDisplayWindow")]
        public static extern LadybugError SetDisplayWindow(int context);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugUpdateTextures")]
        public static extern LadybugError UpdateTextures(
              int context,
              uint uiCameras,
              byte** arpBGRABuffers,
              LadybugPixelFormat pixelFormat);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugDisplayImage")]
        public static extern LadybugError DisplayImage(
             int context,
             LadybugOutputImage imageType);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetSphericalViewParams")]
        public static extern LadybugError SetSphericalViewParams(
             int context,
              float fFOV,
              float fRotX,
              float fRotY,
              float fRotZ,
              float fTransX,
              float fTransY,
              float fTransZ);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugEnableSoftwareRendering")]
        public static extern LadybugError EnableSoftwareRendering(int context, bool enable);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetAntiAliasing")]
        public static extern LadybugError SetAntiAliasing(int context, bool enable);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetTextureIntensityAdjustment")]
        public static extern LadybugError SetTextureIntensityAdjustment(int context, bool enable);
        
        //
        // ladybugstream.h functions
        //
        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugCreateStreamContext")]
        public static extern LadybugError CreateStreamContext(out int context);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugInitializeStreamForWriting")]
        public static extern LadybugError InitializeStreamForWriting(int streamContext,
              string baseFileName,
              int cameraContext,
              StringBuilder fileNameOpened,
              bool async);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugInitializeStreamForWritingEx")]
        public static extern LadybugError InitializeStreamForWritingEx(int streamContext,
              string baseFileName,
              ref LadybugStreamHeadInfo streamInfo,
              string configFilePath,
              bool async);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugWriteImageToStream")]
        public static extern LadybugError WriteImageToStream(int streamContext,
              ref LadybugImage image,
              out double MBytesWritten,
              out uint numImagesWritten);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugInitializeStreamForReading")]
        public static extern LadybugError InitializeStreamForReading(int context,
              string path,
              bool async);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetStreamConfigFile")]
        public static extern LadybugError GetStreamConfigFile(int context, string path);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetStreamHeader")]
        public static extern LadybugError GetStreamHeader(
              int context,
              out LadybugStreamHeadInfo pStreamHeaderInfo,
              string pszAssociatedFileName);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugReadImageFromStream")]
        public static extern LadybugError ReadImageFromStream(
              int context,
              out LadybugImage image);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetStreamNumOfImages")]
        public static extern LadybugError GetStreamNumOfImages(
              int context,
              out uint puiImages);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGoToImage")]
        public static extern LadybugError GoToImage(
              int context,
              uint uiImageNum);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugDestroyStreamContext")]
        public static extern LadybugError DestroyStreamContext(ref int context);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugStopStream")]
        public static extern LadybugError StopStream(int context);

        //
        // ladybugvideo.h functions
        //
        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugCreateVideoContext")]
        public static extern LadybugError CreateVideoContext(out int context);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugDestroyVideoContext")]
        public static extern LadybugError DestroyVideoContext(ref int context);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugOpenVideo")]
        public static extern LadybugError OpenVideo(int context, ref string fileName, ref LadybugH264Option option);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugCloseVideo")]
        public static extern LadybugError CloseVideo(int context);

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugAppendVideoFrame")]
        public static extern LadybugError AppendVideoFrame(int context, ref LadybugProcessedImage image);

        //
        // ladybugImageAdjustment.h functions
        //

        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugSetAdjustmentParameters")]
        public static extern LadybugError SetAdjustmentParameters(int context, ref LadybugAdjustmentParameters param);
    
        [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetAdjustmentParameters")]
        public static extern LadybugError GetAdjustmentParameters(int context, out LadybugAdjustmentParameters param);
    }

    //
    // pgrcameragui.h functions
    //
    unsafe public class CameraGUI
    {
        //private const string LADYBUG_GUI_DLL = "LadybugGUI.dll";
		private const string LADYBUG_GUI_DLL = "Ladybug\\1.10\\LadybugGUI";
		
        [DllImport(LADYBUG_GUI_DLL, EntryPoint = "pgrcamguiCreateContext")]
        public static extern CameraGUIError CreateContext(out int pcontext);


        [DllImport(LADYBUG_GUI_DLL, EntryPoint = "pgrcamguiDestroyContext")]
        public static extern CameraGUIError DestroyContext(int context);

        [DllImport(LADYBUG_GUI_DLL, EntryPoint = "pgrcamguiShowCameraSelectionModal")]
        public static extern CameraGUIError ShowCameraSelectionModal(
                      int context,
                      int camcontext,
                      out int pulSerialNumber,
                      out int pipDialogStatus);

        [DllImport(LADYBUG_GUI_DLL, EntryPoint = "pgrcamguiInitializeSettingsDialog")]
        public static extern CameraGUIError InitializeSettingsDialog(
                      int context,
                      int camcontext);

        [DllImport(LADYBUG_GUI_DLL, EntryPoint = "pgrcamguiToggleSettingsWindowState")]
        public static extern CameraGUIError ToggleSettingsWindowState(
                   int context,
                   IntPtr hwndParent);
    }

    public struct PIXELFORMATDESCRIPTOR
    {
        public ushort nSize;
        public ushort nVersion;
        public uint dwFlags;
        public byte iPixelType;
        public byte cColorBits;
        public byte cRedBits;
        public byte cRedShift;
        public byte cGreenBits;
        public byte cGreenShift;
        public byte cBlueBits;
        public byte cBlueShift;
        public byte cAlphaBits;
        public byte cAlphaShift;
        public byte cAccumBits;
        public byte cAccumRedBits;
        public byte cAccumGreenBits;
        public byte cAccumBlueBits;
        public byte cAccumAlphaBits;
        public byte cDepthBits;
        public byte cStencilBits;
        public byte cAuxBuffers;
        public byte iLayerType;
        public byte bReserved;
        public uint dwLayerMask;
        public uint dwVisibleMask;
        public uint dwDamageMask;
    }

    unsafe public class Win32
    {
        // this can be used to copy from an unmanaged buffer to another unmanaged buffer.
        [DllImport("kernel32.dll")]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint len);
        [DllImport("kernel32.dll")]
        public static extern void RtlMoveMemory(IntPtr dest, IntPtr src, uint len); // works but slow?
    }
	/*
    //
    // This class can be used to access onscreen rendering functionality.
    // By instantiating this, you can create and manage OpenGL context
    // by passing Control on which the onscreen rendering output should
    // be displayed.
    //
    public unsafe class OpenGLBase
    {
        public bool initialize(System.Windows.Forms.Control control)
        {
            graphics = control.CreateGraphics();
            m_hDC = graphics.GetHdc();

            PIXELFORMATDESCRIPTOR pfd = new PIXELFORMATDESCRIPTOR();
            pfd.nSize = (ushort)sizeof(PIXELFORMATDESCRIPTOR);
            pfd.dwFlags = 0x1 | 0x20 | 0x4; //PFD_DOUBLEBUFFER   | PFD_SUPPORT_OPENGL | PFD_DRAW_TO_WINDOW;
            pfd.iPixelType = 0; // PFD_TYPE_RGBA
            pfd.cColorBits = 24;
            pfd.cAlphaBits = 0;
            pfd.cDepthBits = 0;

            int nPixelFormat = GL.ChoosePixelFormat(m_hDC, ref pfd);
            if (nPixelFormat == 0)
                return false;

            if (!GL.SetPixelFormat(m_hDC, nPixelFormat, ref pfd))
                return false;

            m_hRC = GL.wglCreateContext(m_hDC);
            if (m_hRC == IntPtr.Zero)
                return false;

            return true;
        }

        public bool finish()
        {
            if (m_hRC != IntPtr.Zero)
                GL.wglDeleteContext(m_hRC);

            if (graphics == null)
                graphics.Dispose();

            return true;
        }

        public bool bind()
        {
            if (!GL.wglMakeCurrent(m_hDC, m_hRC))
                return false;

            return true;
        }

        public bool unbind()
        {
            if (!GL.wglMakeCurrent(m_hDC, IntPtr.Zero))
                return false;

            return true;
        }

        public bool swapBuffers()
        {
            return GL.SwapBuffers(m_hDC);
        }

        //
        // member variables
        //
        private System.Drawing.Graphics graphics; // drawing surface
        private IntPtr m_hDC; // device context of the graphics
        private IntPtr m_hRC; // OpenGL context (HGLRC)
    }
	 
	 */
    //
    // This class has all the static functions needed to OpenGL functionality
    // Usually, you won't have to use this because OpenGLBase class takes care of this.
    //
    unsafe public class GL
    {
        [DllImport("opengl32.dll")]
        public static extern IntPtr wglCreateContext(IntPtr hDC);

        [DllImport("opengl32.dll")]
        public static extern bool wglMakeCurrent(IntPtr hDC, IntPtr hRC);

        [DllImport("opengl32.dll")]
        public static extern bool wglDeleteContext(IntPtr hRC);

        [DllImport("opengl32.dll")]
        public static extern void glViewport(int x, int y, int width, int height);

        [DllImport("gdi32.dll")]
        public static extern bool SwapBuffers(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern int ChoosePixelFormat(IntPtr hDC, ref PIXELFORMATDESCRIPTOR pfd);

        [DllImport("gdi32.dll")]
        public static extern bool SetPixelFormat(IntPtr hDC, int format, ref PIXELFORMATDESCRIPTOR pfd);

    }
	
	
    //
    // Ported from LadybugFunctors.cpp
    public static class dataFormat
    {

        //=============================================================================
        /**
         * Method:    isImplemented
         * Description : Return true if the format is implemented and
         *               is concrete (return false for LADYBUG_DATAFORMAT_ANY
         *
         * @param format
         *
         * @return
         */
        //=============================================================================
        public static bool isImplemented(LadybugDataFormat format)
        {
            switch (format)
            {
                case LadybugDataFormat.LADYBUG_DATAFORMAT_COLOR_SEP_JPEG8:
                case LadybugDataFormat.LADYBUG_DATAFORMAT_COLOR_SEP_HALF_HEIGHT_JPEG8:
                case LadybugDataFormat.LADYBUG_DATAFORMAT_COLOR_SEP_JPEG12:
                case LadybugDataFormat.LADYBUG_DATAFORMAT_COLOR_SEP_HALF_HEIGHT_JPEG12:
                case LadybugDataFormat.LADYBUG_DATAFORMAT_RAW16:
                case LadybugDataFormat.LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW16:
                case LadybugDataFormat.LADYBUG_DATAFORMAT_RAW12:
                case LadybugDataFormat.LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW12:
                case LadybugDataFormat.LADYBUG_DATAFORMAT_RAW8:
                case LadybugDataFormat.LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW8:
                    return true;
                default:
                    return false;
            }
        }

        //=============================================================================
        /**
         * Method:    isHalfHeight
         * Description :
         *
         * @param format
         *
         * @return True if format is half, false if it is full
         */
        //=============================================================================
        public static bool isHalfHeight(LadybugDataFormat format)
        {
            switch (format)
            {
                case LadybugDataFormat.LADYBUG_DATAFORMAT_COLOR_SEP_HALF_HEIGHT_JPEG8:
                case LadybugDataFormat.LADYBUG_DATAFORMAT_COLOR_SEP_HALF_HEIGHT_JPEG12:
                case LadybugDataFormat.LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW16:
                case LadybugDataFormat.LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW12:
                case LadybugDataFormat.LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW8:
                    return true;
                default:
                    return false;
            }
        }

        //=============================================================================
        /**
         * Method:    isUncompressed
         * Description :
         *
         * @param format
         *
         * @return  True if format is RAW, false if it is JPEG
         */
        //=============================================================================
        public static bool isUncompressed(LadybugDataFormat format)
        {
            System.Diagnostics.Debug.Assert(isImplemented(format));

            return (format == LadybugDataFormat.LADYBUG_DATAFORMAT_RAW8 ||
                format == LadybugDataFormat.LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW8 ||
                format == LadybugDataFormat.LADYBUG_DATAFORMAT_RAW12 ||
                format == LadybugDataFormat.LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW12 ||
                format == LadybugDataFormat.LADYBUG_DATAFORMAT_RAW16 ||
                format == LadybugDataFormat.LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW16);
        }

        //=============================================================================
        /**
         * Method:    isJPEG
         * Description :
         *
         * @param format
         *
         * @return True if format is JPEG, false if it is RAW
         */
        //=============================================================================
        public static bool isJPEG(LadybugDataFormat format)
        {
            System.Diagnostics.Debug.Assert(isImplemented(format));

            return (format == LadybugDataFormat.LADYBUG_DATAFORMAT_COLOR_SEP_JPEG8 ||
                format == LadybugDataFormat.LADYBUG_DATAFORMAT_COLOR_SEP_HALF_HEIGHT_JPEG8 ||
                format == LadybugDataFormat.LADYBUG_DATAFORMAT_COLOR_SEP_JPEG12 ||
                format == LadybugDataFormat.LADYBUG_DATAFORMAT_COLOR_SEP_HALF_HEIGHT_JPEG12);
        }

        //=============================================================================
        /**
         * Method: is12bit
         * Description : Checks if the specified data format is 12 bit. 
         *
         * @param format
         *
         * @return True if format is 1.5 bytes/pixel, false if it is 1.
         */
        //=============================================================================
        public static bool is12bit(LadybugDataFormat format)
        {
            System.Diagnostics.Debug.Assert(isImplemented(format));

            return (format == LadybugDataFormat.LADYBUG_DATAFORMAT_RAW12 ||
                format == LadybugDataFormat.LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW12 ||
                format == LadybugDataFormat.LADYBUG_DATAFORMAT_COLOR_SEP_JPEG12 ||
                format == LadybugDataFormat.LADYBUG_DATAFORMAT_COLOR_SEP_HALF_HEIGHT_JPEG12);
        }

        //=============================================================================
        /**
         * Method:    isHighBitDepth
         * Description : Checks if the specified data format is high bit depth (either 12 or 16 bit)
         *
         * @param format
         *
         * @return True if format is 12 or 16 bits wide, false otherwise.
         */
        //=============================================================================
        public static bool isHighBitDepth(LadybugDataFormat format)
        {
            System.Diagnostics.Debug.Assert(isImplemented(format));

            bool is16Bit = (format == LadybugDataFormat.LADYBUG_DATAFORMAT_RAW16 || 
                format == LadybugDataFormat.LADYBUG_DATAFORMAT_HALF_HEIGHT_RAW16);

            return (is12bit(format) || is16Bit);
        }
    }
}