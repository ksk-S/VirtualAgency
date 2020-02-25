//=============================================================================
// Copyright ｩ 2008 Point Grey Research, Inc. All Rights Reserved.
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
//=============================================================================
// $Id: LadybugAPI.cs,v 1.11.2.1 2011/02/10 17:16:24 hirokim Exp $
//=============================================================================
//
// This file defines the interface that C# programs need to use Ladybug SDK.
// This file must be added to your C# project.
//
//=============================================================================
using System;
using System.Runtime.InteropServices;


namespace LadybugAPI15
{

   //
   // Description:
   //   An enumeration of all possible errors returned by the Ladybug API.
   //
   public enum LadybugError
   {
      // Function completed successfully.
      LADYBUG_OK,
      // General failure.
      LADYBUG_FAILED,
      // Invalid argument passed to the function.
      LADYBUG_INVALID_ARGUMENT,
      // Invalid context passed to the function.
      LADYBUG_INVALID_CONTEXT,
      // The functionality is not implemented with this version of the library.
      LADYBUG_NOT_IMPLEMENTED,
      // The device has already been initialized.
      LADYBUG_ALREADY_INITIALIZED,
      // The stream has already been initialized for reading.
      LADYBUG_ALREADY_INITIALIZED_FOR_READING,
      // The stream has already been initialized for writing.
      LADYBUG_ALREADY_INITIALIZED_FOR_WRITING,
      // Grabbing has already been started.
      LADYBUG_ALREADY_STARTED,
      // Callback is not registered.
      LADYBUG_CALLBACK_NOT_REGISTERED,
      // Callback has already been registered.
      LADYBUG_CALLBACK_ALREADY_REGISTERED,
      // Problem controlling camera.
      LADYBUG_CAMERACONTROL_PROBLEM,
      // Failed to open file.
      LADYBUG_COULD_NOT_OPEN_FILE,
      // Failed to open a device handle.
      LADYBUG_COULD_NOT_OPEN_DEVICE_HANDLE,
      // Memory allocation error.
      LADYBUG_MEMORY_ALLOC_ERROR,
      //  There is not enough space on the disk.
      LADYBUG_ERROR_DISK_NOT_ENOUGH_SPACE,
      //  Stream file has not opened.
      LADYBUG_STREAM_FILE_NOT_OPENED,
      //  Invalid stream file name.
      LADYBUG_INVALID_STREAM_FILE_NAME,
      // Image buffer locked.
      LADYBUG_BUFFER_LOCKED,
      // ladybugGrabimage() not called.
      LADYBUG_NO_IMAGE,
      // Device not initialized.
      LADYBUG_NOT_INITIALIZED,
      // Camera has not been started.
      LADYBUG_NOT_STARTED,
      // Request would exceed maximum bandwidth of the 1394 or 1394b bus.
      LADYBUG_MAX_BANDWIDTH_EXCEEDED,
      // Attached camera is not a Point Grey Research camera.
      LADYBUG_NON_PGR_CAMERA,
      // Invalid video mode or frame rate passed or retrieved.
      LADYBUG_INVALID_MODE,
      // The camera device has reported that it is busy.
      LADYBUG_DEVICE_BUSY,
      // The rectify resolution has not been properly set.
      LADYBUG_NEED_RECTIFY_RESOLUTION,
      // Unknown error.
      LADYBUG_ERROR_UNKNOWN,
      // Function is deprecated - please see documentation.
      LADYBUG_DEPRECATED,
      // The image buffer returned by the camera was too small to contain all of
      // the JPEG image data.
      LADYBUG_IMAGE_TOO_SMALL,
      // Invalid custom image size.
      LADYBUG_INVALID_CUSTOM_SIZE,
      // Operation timed out.
      LADYBUG_TIMEOUT,
      // Too many image buffers are locked by the user.
      LADYBUG_TOO_MANY_LOCKED_BUFFERS,
      // There is a version mismatch between one of the interacting modules: 
      // ladybug.dll, ladybuggui.dll, and the camera driver.
      LADYBUG_VERSION_MISMATCH,
      // No calibration file was found on the Ladybug head unit.
      LADYBUG_CALIBRATION_FILE_NOT_FOUND,
      // A packet has been dropped during transmission.
      LADYBUG_PACKET_DROPPED,
      // An error occurred during JPEG decompression.
      LADYBUG_JPEG_ERROR,
      // An error occurred in JPEG image header.
      LADYBUG_JPEG_HEADER_ERROR,
      // JPEG image buffer is too small to hold image data.
      LADYBUG_JPEG_BUFFER_TOO_SMALL,
      // The compressor did not have enough time to finish compressing the data.
      LADYBUG_JPEG_INCOMPLETE_COMPRESSION,
      // There is no image in this frame.
      LADYBUG_JPEG_NO_IMAGE,
      //The compresser detected a corrupted image.
      LADYBUG_CORRUPTED_IMAGE_DATA,
      // Unable to get address for wglGetExtensionsStringARB. 
      // The graphics card does not support OpenGL extensions.
      LADYBUG_WGLGETEXTENSIONS_STRING_ARB_NOT_FOUND,
      // WGL_ARB_pbuffer functions were not found.
      // Off-screen rendering is not supported by this graphics card.
      LADYBUG_WGLPBUFFER_FUNCTION_NOT_FOUND,
      // An error occurred in off-screen buffer initialization.
      LADYBUG_OFFSCREEN_BUFFER_INIT_ERROR,
      // Unsupported framebuffer format.
      LADYBUG_FRAMEBUFFER_UNSUPPORTED_FORMAT,
      // Framebuffer incomplete.
      LADYBUG_FRAMEBUFFER_INCOMPLETE,
      // GPS device could not be started.
      LADYBUG_GPS_COULD_NOT_BE_STARTED,
      // GPS has not been started.
      LADYBUG_GPS_NOT_STARTED,
      // GPS is started.
      LADYBUG_GPS_STARTED,
      // No GPS data.
      LADYBUG_GPS_NO_DATA,
      // No GPS data for this sentence.
      LADYBUG_GPS_NO_DATA_FOR_THIS_SENTENCE,
      // GPS communication port may be in use.
      LADYBUG_GPS_COMM_PORT_IN_USE,
      // GPS communication port does not exist.
      LADYBUG_GPS_COMM_PORT_DOES_NOT_EXIST,
      // GPS access communication port error.
      LADYBUG_GPS_COMM_PORT_ACCESS_ERROR,
      // OpenGL display list has not initialized.
      LADYBUG_OPENGL_DISPLAYLIST_NOT_INITIALIZED,
      // OpenGL image texture has not updated.
      LADYBUG_OPENGL_TEXTUREIMAGE_NOT_UPDATED,
      // OpenGL device context is invalid.
      LADYBUG_INVALID_OPENGL_DEVICE_CONTEXT,
      // OpenGL rendering context is invalid.
      LADYBUG_INVALID_OPENGL_RENDERING_CONTEXT,
      // OpenGL texture is invalid.
      LADYBUG_INVALID_OPENGL_TEXTURE,
      // There are not enough resources available for image texture.
      LADYBUG_NOT_ENOUGH_RESOURCE_FOR_OPENGL_TEXTURE,
      // The current rendering context failed to share 
      // the display-list space of another rendering context
      LADYBUG_SHARING_DISPLAYLIST_FAILED,
      // Invalid resolution passed to the function.
      LADYBUG_INVALID_RESOLUTION,
      // Invalid data format passed to the function.
      LADYBUG_INVALID_DATAFORMAT,
      // Invalid frame rate passed to the function.
      LADYBUG_INVALID_FRAMERATE,
      // The specified off-screen image is invalid.
      LADYBUG_INVALID_OFFSCREEN_BUFFER_SIZE,
      // The JPEG image data structure is invalid.
      LADYBUG_INVALID_JPEG_IMAGE_STRUCTURE,
      // The requested job is still on-going.
      LADYBUG_STILL_WORKING,
      // The PGR stream is corrupted and cannot be corrected.
      LADYBUG_CORRUPTED_PGR_STREAM,

      // Number of errors
      LADYBUG_NUM_LADYBUG_ERRORS,

      // Unused member to force this enumeration to compile to 32 bits.
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
   public enum  LadybugProperty
   {
      // The brightness property.
      LADYBUG_BRIGHTNESS,
      // The auto exposure property.
      LADYBUG_AUTO_EXPOSURE,
      // The sharpness property. Not supported
      LADYBUG_SHARPNESS,
      // The white balance property.
      LADYBUG_WHITE_BALANCE,
      // The hue property. Not supported
      LADYBUG_HUE,
      // The saturation property. Not supported
      LADYBUG_SATURATION,
      // The gamma property.
      LADYBUG_GAMMA,
      // The iris property. Not supported
      LADYBUG_IRIS,
      // The focus property. Not supported
      LADYBUG_FOCUS,
      // The zoom property. Not supported
      LADYBUG_ZOOM,
      // The pan property.  This property controls the mechanism used by the
      // camera when it is in auto-exposure mode (at least one of auto-gain and 
      // auto-shutter is on).  The first six settings of this property (0-5)
      // allow the user to select which image to use for auto-exposure.
      // For instance, setting the Pan property to 5 will result in the camera
      // adjusting the gain and/or shutter based on the contents of camera 5.  
      // There are two additional settings -
      // 6 - calculate exposure based on the brightest image (not very stable).
      // 7 - calculate exposure based on all of the images.
      LADYBUG_PAN,
      // The tilt property. Not supported
      LADYBUG_TILT,
      // The shutter property.
      LADYBUG_SHUTTER,
      // The gain property.
      LADYBUG_GAIN,
      // The camera heads frame rate
      LADYBUG_FRAME_RATE,
      // Unused member to force this enumeration to compile to 32 bits.
      LADYBUG_PROPERTY_FORCE_QUADLET = 0x7FFFFFFF,

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
      // This format involves interleaving every image from each of the 6 
      // image sensors on a pixel by pixel basis.  Each pixel is in its raw 
      // 8bpp format. This is the only 6 image format supported by the Ladybug.
      // This format is not supported by Ladybug2.
      LADYBUG_DATAFORMAT_INTERLEAVED,

      // This format produces a single image buffer that has each sensor's image 
      // one after the other. Again, each pixel is in its raw 8bpp format.  This 
      // format is only supported by the Ladybug2.  This format is not supported
      // by the original Ladybug.
      LADYBUG_DATAFORMAT_SEQUENTIAL,

      // This format is similar to the LADYBUG_DATAFORMAT_SEQUENTIAL 
      // except that the entire buffer is JPEG compressed.  This format is 
      // intended for use with cameras that have black and white sensors. It is 
      // not supported by the original Ladybug.
      LADYBUG_DATAFORMAT_SEQUENTIAL_JPEG,

      // In addition to separating the images sequentially, this format separates 
      // each individual image into its 4 individual Bayer channels (Green, Red,
      // Blue and Green - not necessarily in that order). This format is only 
      // supported by Ladybug2.
      LADYBUG_DATAFORMAT_COLOR_SEP_SEQUENTIAL,

      // This format is very similar to 
      // LADYBUG_DATAFORMAT_COLOR_SEP_SEQUENTIAL except that the transmitted
      // buffer is JPEG compressed. This format is only supported by Ladybug2.
      LADYBUG_DATAFORMAT_COLOR_SEP_SEQUENTIAL_JPEG,

      // This format is similar to LADYBUG_DATAFORMAT_SEQUENTIAL. The height
      // of the image is only half of that in LADYBUG_DATAFORMAT_SEQUENTIAL
      // format. This format is only supported by Ladybug3.
      LADYBUG_DATAFORMAT_SEQUENTIAL_HALF_HEIGHT,

      // This format is similar to LADYBUG_DATAFORMAT_COLOR_SEP_SEQUENTIAL_JPEG.
      // The height of each individual Bayer channel image is only one fourth 
      // of the original Bayer channel image. This format is only supported by 
      // Ladybug3.
      LADYBUG_DATAFORMAT_COLOR_SEP_SEQUENTIAL_HALF_HEIGHT_JPEG,

      // The number of possible data formats.
      LADYBUG_NUM_DATAFORMATS,

      // Hook for "any usable video mode".
      LADYBUG_DATAFORMAT_ANY,

      // Unused member to force this enumeration to compile to 32 bits.
      LADYBUG_DATAFORMAT_FORCE_QUADLET = 0x7FFFFFFF,

   };

    
   public enum LadybugSaveFileFormat
   {
      // 8-bit greyscale .PGM.
      LADYBUG_FILEFORMAT_PGM,
      // 24 bit .PPM.
      LADYBUG_FILEFORMAT_PPM,
      // 24 bit .BMP.
      LADYBUG_FILEFORMAT_BMP,
      // JPEG image.
      LADYBUG_FILEFORMAT_JPG,
      // PNG image.  Not supported yet.
      LADYBUG_FILEFORMAT_PNG,
      // EXIF image. 
      // GPS information will be stored in EXIF tags if it is present LadybugProcessedImage.
      LADYBUG_FILEFORMAT_EXIF,

      // Unused member to force this enumeration to compile to 32 bits.
      LADYBUG_FILEFORMAT_FORCE_QUADLET = 0x7FFFFFFF,

   };



   public enum LadybugResolution
   {
      // 128x96 pixels. Not supported.
      LADYBUG_RESOLUTION_128x96,
      // 256x192 pixels. Not supported.
      LADYBUG_RESOLUTION_256x192,
      // 512x384 pixels. Not supported.
      LADYBUG_RESOLUTION_512x384,
      // 640x480 pixels. Not supported.
      LADYBUG_RESOLUTION_640x480,
      // 1024x768 pixels. Ladybug2 camera.
      LADYBUG_RESOLUTION_1024x768,
      // 1216x1216 pixels. Not supported.
      LADYBUG_RESOLUTION_1216x1216,
      // 1616x1216 pixels. Not supported.
      LADYBUG_RESOLUTION_1616x1216,
      // 1600x1200 pixels, Not supported.
      LADYBUG_RESOLUTION_1600x1200,
      // 1616x1232 pixels. Ladybug3 camera. 
      LADYBUG_RESOLUTION_1616x1232,

      // Number of possible resolutions.
      LADYBUG_NUM_RESOLUTIONS,
      // Hook for "any usable resolution".
      LADYBUG_RESOLUTION_ANY,

      // Unused member to force this enumeration to compile to 32 bits.
      LADYBUG_RESOLUTION_FORCE_QUADLET = 0x7FFFFFFF,

   };

   public enum LadybugStippledFormat
   {
      // indicates a BGGR image.
      LADYBUG_BGGR,
      // indicates a GBRG image.
      LADYBUG_GBRG,
      // indicates a GRBG image.
      LADYBUG_GRBG,
      // indicates an RGGB image.
      LADYBUG_RGGB,
      // indicates the default stipple format for the camera.
      LADYBUG_DEFAULT,

      // Unused member to force this enumeration to compile to 32 bits.
      LADYBUG_STIPPLED_FORCE_QUADLET = 0x7FFFFFFF,

   };

   public enum LadybugPixelFormat
   {
      // 8 bit of mono
      LADYBUG_MONO8 = 0x00000001,
      // 16 bit mono
      LADYBUG_MONO16 = 0x00000020,
      // 8 bit raw data
      LADYBUG_RAW8 = 0x00000200,
      // 16 bit raw data
      LADYBUG_RAW16 = 0x00000400,
      // 24 bit BGR
      LADYBUG_BGR = 0x10000001,
      // 32 bit BGRU
      LADYBUG_BGRU = 0x10000002,
      // Unused member to force this enum to compile to 32 bits.
      LADYBUG_PIXELFORMAT_FORCE_QUADLET = 0x7FFFFFFF,
   };

   public enum LadybugFramerate
   {
      // 1.875 fps.
      LADYBUG_FRAMERATE_1_875,
      // 3.75 fps.
      LADYBUG_FRAMERATE_3_75,
      // 7.5 fps.
      LADYBUG_FRAMERATE_7_5,
      // 15 fps.
      LADYBUG_FRAMERATE_15,
      // 30 fps.
      LADYBUG_FRAMERATE_30,
      // Number of possible camera frame rates.
      LADYBUG_NUM_FRAMERATES,
      // Hook for "any usable frame rate".
      LADYBUG_FRAMERATE_ANY,
      // Unused member to force this enumeration to compile to 32 bits.
      LADYBUG_FRAMERATE_FORCE_QUADLET = 0x7FFFFFFF,

   };

   public enum LadybugColorProcessingMethod
   {
      // Disable color processing.
      LADYBUG_DISABLE,

      // Edge sensing de-mosaicing - This is the most accurate method
      //  that can still keep up with the camera's frame rate.
      LADYBUG_EDGE_SENSING,

      // Nearest neighbor de-mosaicing - This algorithm is significantly
      // faster than edge sensing, at the cost of accuracy.
      LADYBUG_NEAREST_NEIGHBOR,

      // Nearest neighbour de-mosaicing (fast) - Faster, less accurate 
      // nearest neighbor de-mosaicing.
      LADYBUG_NEAREST_NEIGHBOR_FAST,

      //  Rigorous de-mosaicing - This provides the best quality colour
      //  reproduction.  This method very processor intensive and may
      //  not keep up with the camera's frame rate.  Best used for
      //  offline processing where accurate colour reproduction is required. 
      LADYBUG_RIGOROUS,

      // Quarter image downsampling - Only used for compressor images. Instead
      // of color processing, 4 bayer pixels are combined to form one colour
      // pixel.  This allows for fast previews.
      LADYBUG_DOWNSAMPLE4,

      // Mono - This processing method only uses the green color channel to
      // generate grey scale Ladybug images. It is designed for fast previews of
      // compressed JPEG image streams. 
      LADYBUG_MONO,
	
      // High quality linear interpolation - This algorithm provides similar
      // results to Rigorous, but is up to 30 times faster.
      LADYBUG_HQLINEAR,

      // Unused member to force this enumeration to compile to 32 bits.
      LADYBUG_COLOR_FORCE_QUADLET = 0x7FFFFFFF,

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

      // The data format of associated image buffer contained in pData. 
      public LadybugDataFormat dataFormat;

      // The per-sensor resolution of the returned image. 
      public LadybugResolution resolution;

      // Timestamp of this image.
      public LadybugTimestamp timeStamp;

      // Image information for this image. 
      public LadybugImageInfo imageInfo;

      // Pointer to the image data.  The format is defined by dataFormat. 
      public byte* pData;

      // Indicates whether the raw image data is stippled or not. 
      public bool bStippled;

      // Real data size, in bytes, of the data pointed to by pData.  Useful for
      // non-constant sizes (JPEG images).
      public uint uiDataSizeBytes;

      // The sequence number of the image.  This number is generated in the 
      // driver and sequential images should have a difference of one.  If
      // the difference is greater than one, it indicates the number of missed
      // images since the last lock image call.
      public uint uiSeqNum;

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
      public fixed uint reservedSpace[212];
      public fixed uint ulOffsetTable[512];
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

      // Reserved
      public fixed uint ulReserved[8];
   };

    unsafe public struct LadybugStabilizationParams
    {
        public int iNumTemplates;
        public int iMaximumSearchRegion;
        public double dDecayRate;
        public fixed int reserved[28];
    };

        
   //
   // This class defines static functions to access most of the 
   // Ladybug APIs defined in ladybug.h, ladybuggeom.h, ladybugrenderer.h
   // and ladybugstream.h.
   //
   unsafe public partial class Ladybug
   {
		
		private const string LADYBUG_DLL = "Ladybug\\1.5\\ladybug";
      //private const string LADYBUG_DLL = "ladybug.dll";
      public const int LADYBUG_NUM_CAMERAS = 6;

      //
      // ladybug.h functions
      //
      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugCreateContext")]
      public static extern LadybugError CreateContext(out int context);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugErrorToString")]
      public static extern string ErrorToString(LadybugError errorCode);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetColorTileFormat")]
      public static extern LadybugError SetColorTileFormat(
	    int context,
	    LadybugStippledFormat format);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugConvertToMultipleBGRU32")]
      public static extern LadybugError ConvertToMultipleBGRU32(
		  int context,
		  ref LadybugImage pImage,
		  byte** arpDestBuffers,
		  out LadybugImageInfo pImageInfo);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetColorProcessingMethod")]
      public static extern LadybugError SetColorProcessingMethod(
		  int context,
		  LadybugColorProcessingMethod method);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGetColorProcessingMethod")]
      public static extern LadybugError GetColorProcessingMethod(
		  int context,
		  out LadybugColorProcessingMethod method);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugStart")]
      public static extern LadybugError Start(
		  int context,
		  LadybugResolution resolution,
		  LadybugDataFormat format,
		  LadybugFramerate frameRate);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugStartEx")]
      public static extern LadybugError StartEx(
		  int context,
		  LadybugResolution resolution,
		  LadybugDataFormat format,
		  uint packetSize,
		  uint bufferSize);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugStartLockNext")]
      public static extern LadybugError StartLockNext(
		  int context,
		  LadybugResolution resolution,
		  LadybugDataFormat format,
		  LadybugFramerate frameRate);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugStartLockNextEx")]
      public static extern LadybugError StartLockNextEx(
		  int context,
		  LadybugResolution resolution,
		  LadybugDataFormat format,
		  uint packetSize,
		  uint bufferSize);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGrabImage")]
      public static extern LadybugError GrabImage(
				     int context,
				     out LadybugImage pImage);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugLockNext")]
      public static extern LadybugError LockNext(
				     int context,
				     out LadybugImage pImage);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugUnlock")]
      public static extern LadybugError Unlock(
				     int context,
				     uint bufferIndex);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugUnlockAll")]
      public static extern LadybugError UnlockAll( int context);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugInitializeFromIndex")]
      public static extern LadybugError InitializeFromIndex(int context, uint ulDevice);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugInitializeFromSerialNumber")]
      public static extern LadybugError InitializeFromSerialNumber(int context, int serialNumber);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugStop")]
      public static extern LadybugError Stop(int context);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugDestroyContext")]
      public static extern LadybugError DestroyContext(ref int context);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetProperty")]
      public static extern LadybugError SetProperty(int context, 
						   LadybugProperty property,
						   int valueA,
						   int valueB,
						   bool auto);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetAbsProperty")]
      public static extern LadybugError SetAbsProperty(int context,
						   LadybugProperty property,
						   float valueA);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGetProperty")]
      public static extern LadybugError GetProperty(int context,
						   LadybugProperty property,
						   out int valueA,
						   out int valueB,
						   out bool auto);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGetAbsProperty")]
      public static extern LadybugError GetAbsProperty(int context,
						   LadybugProperty property,
						   out float valueA);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGetRegister")]
      public static extern LadybugError GetRegister(int context,
						   uint register,
						   out uint value);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetRegister")]
      public static extern LadybugError SetRegister(int context,
						   uint register,
						   uint value);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetJPEGQuality")]
      public static extern LadybugError SetJPEGQuality(int context,
						   int quality);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGetJPEGQuality")]
      public static extern LadybugError GetJPEGQuality(int context,
						   out int quality);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetAutoJPEGQualityControlFlag")]
      public static extern LadybugError SetAutoJPEGQualityControlFlag(int context,
						   bool autoJPEGQualityControl);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGetAutoJPEGQualityControlFlag")]
      public static extern LadybugError GetAutoJPEGQualityControlFlag(int context,
						   out bool autoJPEGQualityControl);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetFalloffCorrectionFlag")]
      public static extern LadybugError SetFalloffCorrectionFlag(int context,
                         bool correctFalloff);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetFalloffCorrectionAttenuation")]
      public static extern LadybugError SetFalloffCorrectionAttenuation(int context,
                        float attenuationFraction);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugEnableImageStabilization")]
      public static extern LadybugError EnableImageStabilization(int context,
                         bool enable,
                         ref LadybugStabilizationParams stabilizationParams);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSaveImage")]
      public static extern LadybugError SaveImage(int context,
                         ref LadybugProcessedImage processedImage,
                         string path,
                         LadybugSaveFileFormat format,
                         bool async);

      [DllImport(LADYBUG_DLL, EntryPoint = "ladybugDoOneShotAutoWhiteBalance")]
      public static extern LadybugError DoOneShotAutoWhiteBalance(int context);

      [DllImport(LADYBUG_DLL, EntryPoint = "ladybugGetOneShotAutoWhiteBalanceStatus")]
      public static extern LadybugError GetOneShotAutoWhiteBalanceStatus(int context);


      //
      // ladybuggeom.h functions
      //
      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugInitializeAlphaMasks")]
      public static extern LadybugError InitializeAlphaMasks(
		      int context,
		      uint uiCols,
		      uint uiRows);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugLoadAlphaMasks")]
      public static extern LadybugError LoadAlphaMasks(
		      int context,
		      uint uiCols,
		      uint uiRows);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugLoadConfig")]
      public static extern LadybugError LoadConfig(int context, string path);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetAlphaMasking")]
      public static extern LadybugError SetAlphaMasking(int context, bool enable);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSet3dMapRotation")]
      public static extern LadybugError Set3dMapRotation(int context, double dRx, double dRy, double dRz);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGet3dMapRotation")]
      public static extern LadybugError Get3dMapRotation(int context, out double dRx, out double dRy, out double dRz);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetBlendingParams")]
      public static extern LadybugError SetBlendingParams(int context, double dMaxBlendingWidth);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGetBlendingParams")]
      public static extern LadybugError GetBlendingParams(int context, out double dMaxBlendingWidth);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGetCameraUnitFocalLength")]
      public static extern LadybugError GetCameraUnitFocalLength(int context, int camera, out double focalLength);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGetCameraUnitImageCenter")]
      public static extern LadybugError GetCameraUnitImageCenter(int context, int camera, out double centerX, out double centerY);

      //
      // ladybugrenderer.h functions
      //
      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugConfigureOutputImages")]
      public static extern LadybugError ConfigureOutputImages(
		      int context,
		      uint uiImageTypes);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetOffScreenImageSize")]
      public static extern LadybugError SetOffScreenImageSize(
		      int context,
		      LadybugOutputImage imageType,
		      uint uiCols,
		      uint uiRows);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugRenderOffScreenImage")]
      public static extern LadybugError RenderOffScreenImage(
	       int context,
	       LadybugOutputImage imageType,
	       out LadybugProcessedImage pImage);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetDisplayWindow")]
      public static extern LadybugError SetDisplayWindow(int context);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugUpdateTextures")]
      public static extern LadybugError UpdateTextures(
			int context,
			uint uiCameras,
			byte** arpBGRABuffers);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugDisplayImage")]
      public static extern LadybugError DisplayImage(
	       int context,
	       LadybugOutputImage imageType);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugSetSphericalViewParams")]
      public static extern LadybugError SetSphericalViewParams(
	       int context,
			float fFOV,
			float fRotX,
			float fRotY,
			float fRotZ,
			float fTransX,
			float fTransY,
			float fTransZ);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugEnableSoftwareRendering")]
      public static extern LadybugError EnableSoftwareRendering(int context, bool enable);


      //
      // ladybugstream.h functions
      //
      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugCreateStreamContext")]
      public static extern LadybugError CreateStreamContext(out int context);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugInitializeStreamForWriting")]
      public static extern LadybugError InitializeStreamForWriting(  int streamContext, 
            string baseFileName,
            int cameraContext,
            [MarshalAs(UnmanagedType.LPStr)] string fileNameOpened, 
            bool async);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugInitializeStreamForWritingEx")]
      public static extern LadybugError InitializeStreamForWritingEx(	int streamContext,
            string baseFileName,
            ref LadybugStreamHeadInfo streamInfo,
            string configFilePath,
            bool async);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugWriteImageToStream")]
      public static extern LadybugError WriteImageToStream(    int streamContext, 
            ref LadybugImage image,
            out double MBytesWritten,
            out uint numImagesWritten);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugInitializeStreamForReading")]
      public static extern LadybugError InitializeStreamForReading(int context, 
            string path, 
            bool async);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGetStreamConfigFile")]
      public static extern LadybugError GetStreamConfigFile(int context, string path);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGetStreamHeader")]
      public static extern LadybugError GetStreamHeader(
            int context,
            out LadybugStreamHeadInfo pStreamHeaderInfo,
            string pszAssociatedFileName);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugReadImageFromStream")]
      public static extern LadybugError ReadImageFromStream(
            int context,
            out LadybugImage image);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGetStreamNumOfImages")]
      public static extern LadybugError GetStreamNumOfImages(
            int context,
            out uint puiImages);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugGoToImage")]
      public static extern LadybugError GoToImage(
            int context,
            uint uiImageNum);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugDestroyStreamContext")]
      public static extern LadybugError DestroyStreamContext(ref int context);

      [DllImport( LADYBUG_DLL, EntryPoint = "ladybugStopStream")]
      public static extern LadybugError StopStream(int context);

   }

   //
   // pgrcameragui.h functions
   //
   unsafe public class CameraGUI
   {
		
	 
			private const string LADYBUG_GUI_DLL = "Ladybug\\1.5\\ladybugGUI";
	 
	  
      //private const string LADYBUG_GUI_DLL = "LadybugGUI.dll";

      [DllImport( LADYBUG_GUI_DLL, EntryPoint = "pgrcamguiCreateContext")]
      public static extern CameraGUIError CreateContext(out int pcontext);


      [DllImport( LADYBUG_GUI_DLL, EntryPoint = "pgrcamguiDestroyContext")]
      public static extern CameraGUIError DestroyContext(int context);

      [DllImport( LADYBUG_GUI_DLL, EntryPoint = "pgrcamguiShowCameraSelectionModal")]
      public static extern CameraGUIError ShowCameraSelectionModal(
					int       context,
					int   camcontext,
					out int        pulSerialNumber,
					out int               pipDialogStatus );

      [DllImport( LADYBUG_GUI_DLL, EntryPoint = "pgrcamguiInitializeSettingsDialog")]
      public static extern CameraGUIError InitializeSettingsDialog(
					int context,
					int camcontext);

      [DllImport( LADYBUG_GUI_DLL, EntryPoint = "pgrcamguiToggleSettingsWindowState")]
      public static extern CameraGUIError ToggleSettingsWindowState(
				 int context,
				 IntPtr		      hwndParent );
   }
	
	unsafe public class MyLadybug
	{
		
		private const string RENDER_DLL = "RenderingPlugin"; // must be in Assets\\Plugins 
		
		[DllImport( RENDER_DLL, EntryPoint = "NativeConvertToMultipleBGRU32")]
      	public static extern LadybugError NativeConvertToMultipleBGRU32(
		  int context,
		  ref LadybugImage pImage,
		  byte** arpDestBuffers,
		  out LadybugImageInfo pImageInfo);
		
		[DllImport( RENDER_DLL, EntryPoint = "NativeUpdateTextures")]
     	 public static extern LadybugError NativeUpdateTextures(
			int context,
			uint uiCameras,
			byte** arpBGRABuffers);
		
		[DllImport( RENDER_DLL, EntryPoint = "NativeConvertAndUpdateTexture")]
      	public static extern LadybugError NativeConvertAndUpdateTexture(
		  int context,
  		  uint uiCameras,
		  ref LadybugImage pImage,
		  out LadybugImageInfo pImageInfo);
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
	
	
}
