FROM rust:1.56 as build

# create a new empty shell project
RUN USER=root cargo new --bin reststops
WORKDIR /reststops

# copy over your manifests
COPY ./Cargo.lock ./Cargo.lock
COPY ./Cargo.toml ./Cargo.toml

# this build step will cache your dependencies
RUN cargo build --release
RUN rm src/*.rs

# copy your source tree
COPY ./src ./src

# build for release
RUN rm ./target/release/deps/reststops*
RUN cargo build --bin api --release

# our final base
FROM debian:buster-slim

RUN apt update
RUN apt install openssl apt-transport-https ca-certificates -y

# copy the build artifact from the build stage
COPY --from=build /reststops/target/release/api .

# set the startup command to run your binary
ENV ROCKET_ADDRESS=0.0.0.0
EXPOSE 8000
CMD ["./api"]