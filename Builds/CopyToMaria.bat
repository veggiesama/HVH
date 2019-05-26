set source="C:\Users\Veggiesama\Documents\Unity\HVH\Builds"
set destination="\\Maria-pc\hvh"

del /f/q/s %destination%
xcopy %source% %destination% /e /y