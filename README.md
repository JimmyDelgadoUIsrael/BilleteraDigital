# BilleteraDigital


---Subir cambios locales de Jimmy a remoto----
##Asegúrate de estar en la rama Jimmy
git checkout Jimmy
##Verifica qué archivos están modificados
git status
##Añade y haz commit de tus cambios (si no lo has hecho)
git add .
git commit -m "Tu mensaje descriptivo del cambio"
##Sube la rama al repositorio remoto
git push origin Jimmy

----Pasar los cambios de Jimmy a master-----
##Cambia a master
git checkout master
##Asegúrate de tener la última versión de master
git pull origin master
##Fusiona Jimmy en master
git merge Jimmy
##Sube los cambios al remoto
git push origin master

----Actualizar todas las ramas remotas con los cambios de master---
git checkout Jimmy
git pull origin Jimmy
git merge master
git push origin Jimmy

