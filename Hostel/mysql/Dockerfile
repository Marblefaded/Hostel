FROM mysql:5.7.43

EXPOSE 3306
RUN echo "Asia/Novosibirsk" > /etc/timezone \
    && rm -f /etc/localtime \
    && dpkg-reconfigure -f noninteractive tzdata \
    && date
COPY *.sql /docker-entrypoint-initdb.d/