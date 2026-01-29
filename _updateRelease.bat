set ES=.
set OUT=.\_Release\

mkdir %OUT%

copy %ES%\ESystem\bin\debug\net8.0\esystem.dll %OUT%
copy %ES%\ESystem\bin\debug\net8.0\esystem.pdb %OUT%

copy %ES%\ESystem.WPF\bin\debug\net8.0-windows7.0\esystem.wpf.dll %OUT%
copy %ES%\ESystem.WPF\bin\debug\net8.0-windows7.0\esystem.wpf.pdb %OUT%

REM copy %ES%\EXmlLib\bin\debug\net8.0\EXmlLib.dll %OUT%
REM copy %ES%\EXmlLib\bin\debug\net8.0\EXmlLib.pdb %OUT%

copy %ES%\EXmlLib2\bin\debug\net8.0\EXmlLib2.dll %OUT%
copy %ES%\EXmlLib2\bin\debug\net8.0\EXmlLib2.pdb %OUT%

pause