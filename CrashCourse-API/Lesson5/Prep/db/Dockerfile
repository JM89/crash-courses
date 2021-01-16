FROM mcr.microsoft.com/mssql/server:2017-latest-ubuntu

ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=VerySecret1234!

RUN mkdir -p /usr/app
WORKDIR /usr/app
COPY . /usr/app
RUN chmod +x /usr/app/run-initialization.sh

CMD /bin/bash ./entrypoint.sh
