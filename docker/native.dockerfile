FROM ubuntu:18.04

RUN apt-get update && \
    apt-get install -y git wget gnupg make curl unzip tar

RUN echo "deb http://apt.llvm.org/bionic/ llvm-toolchain-bionic-7 main" | tee /etc/apt/sources.list.d/llvm.list
RUN wget -O - https://apt.llvm.org/llvm-snapshot.gpg.key | apt-key add -

RUN apt-get update && \
    apt-get install -y clang-7 lldb-7 lld-7

ENV CXX=clang++-7
ENV CC=clang-7

RUN mkdir -p /opt

# vcpkg
RUN cd /opt && \
    git clone https://github.com/Microsoft/vcpkg.git && \
    cd vcpkg && \
    ./bootstrap-vcpkg.sh

RUN /opt/vcpkg/vcpkg integrate install

# libraries
RUN cd /opt/vcpkg && \
    ./vcpkg install nlohmann-json re2

# cmake
RUN apt-get remove -y cmake && \
    wget -nv -O /tmp/cmake.sh https://github.com/Kitware/CMake/releases/download/v3.14.5/cmake-3.14.5-Linux-x86_64.sh && \
    sh /tmp/cmake.sh --prefix=/usr/local --exclude-subdir --skip-license
