@echo ---------------------------------
@echo Building fonts
@echo ---------------------------------

del *.tga

@FORFILES /M *.bmfc /C "cmd /c echo ...@file"

@FORFILES /M *.bmfc /C "cmd /c bmfont.com -c @file -o @fname"

@echo Done!
