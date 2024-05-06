set ES=.
set OUT=.\_Release\

mkdir %OUT%

copy %ES%\ESystem\bin\debug\net6.0\esystem.dll %OUT%
copy %ES%\ESystem\bin\debug\net6.0\esystem.pdb %OUT%

copy %ES%\ELogging\bin\Debug\net6.0\ELogging.dll %OUT%
copy %ES%\ELogging\bin\debug\net6.0\ELogging.pdb %OUT%
copy %ES%\ELogging\bin\debug\net6.0\ELogging.deps.json %OUT%

copy %ES%\ESystem.WPF\bin\debug\net6.0-windows\esystem.wpf.dll %OUT%
copy %ES%\ESystem.WPF\bin\debug\net6.0-windows\esystem.wpf.pdb %OUT%

copy %ES%\EXmlLib\bin\debug\net6.0-windows\EXmlLib.dll %OUT%
copy %ES%\EXmlLib\bin\debug\net6.0-windows\EXmlLib.pdb %OUT%

pause