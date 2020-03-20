#!/bin/bash
set -euxo pipefail

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"
VERSION=1.13.4

mkdir -p $DIR/../deploy/linux
cp $DIR/../integrations.json $DIR/../src/Datadog.Trace.ClrProfiler.Native/bin/Debug/x64/
cp $DIR/../createLogPath.sh $DIR/../src/Datadog.Trace.ClrProfiler.Native/bin/Debug/x64/

cd $DIR/../deploy/linux
for pkgtype in $PKGTYPES ; do
    fpm \
        -f \
        -s dir \
        -t $pkgtype \
        -n lightstep-dotnet-apm \
        -v $VERSION \
        $(if [ $pkgtype != 'tar' ] ; then echo --prefix /opt/datadog ; fi) \
        --chdir $DIR/../src/Datadog.Trace.ClrProfiler.Native/bin/Debug/x64 \
        netstandard2.0/ \
        Datadog.Trace.ClrProfiler.Native.so \
        integrations.json \
        createLogPath.sh
done

gzip -f datadog-dotnet-apm.tar

if [ -z "${MUSL-}" ]; then
  mv lightstep-dotnet-apm.tar.gz lightstep-dotnet-apm-$VERSION.tar.gz
else
  mv lightstep-dotnet-apm.tar.gz lightstep-dotnet-apm-$VERSION-musl.tar.gz
fi
