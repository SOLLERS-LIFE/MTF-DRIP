MTF DRIP v 1.04 (Based on my own template for database interfaces I use in production, but significantly simplified and bastardized for clarity ;-) )

TO INSTALL ON WINDOWS 10+ (Tested Windows 10, Windows 11, Windows Server 2019)

Please login with administrator privileges.

Download MariaDB for Windows from https://mariadb.org/download/?t=mariadb&p=mariadb&r=10.6.5&os=windows&cpu=x86_64&pkg=msi&m=ukfast

Keep all options unchanged. Unfortunately, MariaDB mirrors are too slow very often, as such I don’t use powershell script – it can crush any time.

To install and prepare – open your favourite Windows shell, set directory with the downloaded file as current and execute two following command lines:

> msiexec /i "mariadb-10.6.5-winx64.msi" /qb PORT="3306" ALLOWREMOTEROOTACCESS="true" PASSWORD="55555" SERVICENAME="MySQL"

> "C:\Program Files\MariaDB 10.6\bin\mysql.exe" -uroot -p55555 -e "create database if not exists test;"

Then clone repository, build with Debug or Release. Set lunch settings as “Kestrel-development” to run on the local machine. “Kestrel-production” is for container deployment to Docker hub.

TO INSTALL ON LINUX (Tested RHEL 8.4, Oracle Linux 8.4) with podman-compose

In your favourite shell:

$ sudo dnf install podman podman-compose
$ sudo systemctl enable podman.socket --now

Please create directory with any name. Copy file container-compose.yml from solution directory to the directory created, set this as a current directory with cd, then

$ podman-compose up -d 1podfw

Then in your browser go to http://127.0.0.1:8085
The pod has additional interface at http://127.0.0.1:8080 (phpMyAdmin) for a case if database maintenance is required (server: db user: root password: 55555)

 

