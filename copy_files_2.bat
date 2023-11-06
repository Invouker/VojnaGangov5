@echo off
robocopy "E:\Github\Vojna5Gangov\build" "E:\Github\Vojna5Gangov\output" Server.net.dll /NDL
robocopy "E:\Github\Vojna5Gangov\build" "E:\Github\Vojna5Gangov\output" Client.net.dll /NDL
exit 0

