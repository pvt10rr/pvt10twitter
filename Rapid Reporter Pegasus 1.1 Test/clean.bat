@echo "Copying executable"
@echo -----------------
@copy "Rapid Reporter\bin\Release\RapidReporter.exe" .
@echo.
@echo "Deleting obj&bin"
@echo -----------------
@rmdir "Rapid Reporter\obj\" /s /q
@rmdir "Rapid Reporter\obj\" /s /q
@rmdir "Rapid Reporter\bin\" /s /q
@rmdir "Rapid Reporter\bin\" /s /q
@echo.
@echo "Deleting csv, rtf, jpg, htm"
@echo -----------------
@del *.csv
@del *.rtf
@del *.jpg
@del *.htm
@echo.
pause
