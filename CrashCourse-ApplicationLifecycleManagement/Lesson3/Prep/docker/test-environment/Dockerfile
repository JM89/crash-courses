FROM mcr.microsoft.com/dotnet/runtime:3.1
RUN apt-get update && \
    apt-get install -y netcat ssh iputils-ping && \
    mkdir /var/run/sshd && \
    chmod 0755 /var/run/sshd && \
    useradd -p $(openssl passwd -1 u2password) --create-home --shell /bin/bash --groups sudo u2

EXPOSE 22

CMD ["/usr/sbin/sshd", "-D"]