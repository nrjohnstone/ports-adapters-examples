FROM mysql:5.6

ARG FLYWAY_VERSION="4.2.0"
ARG FLYWAY_CHECKSUM="f474d22d8107932579cf2c427619b8183f062803"

ENV DATABASE_HOST="127.0.0.1"
ENV DATABASE_PORT="3306"

RUN apt-get -y update \
 && apt-get -y install curl bash netcat \ 
 && apt-get install -y dos2unix \
 && rm -rf /var/lib/apt/lists/*

RUN mkdir -p /opt/flyway \
 && curl -s -o /tmp/flyway.tar.gz https://repo1.maven.org/maven2/org/flywaydb/flyway-commandline/${FLYWAY_VERSION}/flyway-commandline-${FLYWAY_VERSION}-linux-x64.tar.gz \
 && echo "${FLYWAY_CHECKSUM} /tmp/flyway.tar.gz" | sha1sum -c --quiet - \
 && tar -xz -f /tmp/flyway.tar.gz --strip-components=1 -C /opt/flyway \
 && rm /tmp/flyway.tar.gz

ENV PATH=/opt/flyway:$PATH

COPY ./run-flyway.sh /

RUN dos2unix ./run-flyway.sh 

WORKDIR /database

# runs the flyway migrations in the background and starts the mysql server.
# the run-flyway.sh script polls for the database to be started.
CMD /run-flyway.sh& docker-entrypoint.sh mysqld