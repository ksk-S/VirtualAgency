set n=8
set InDir=ScreenshotOutput
set OutDir=Videos

mkdir %OutDir%

for /l %%i in (1,1,%n%) do (
  echo process video %%i...%OutDir%
  ffmpeg -i "..\%InDir%\video%%i\shot%%04d.png" -q:v 1 %OutDir%\L%%i.avi
)
