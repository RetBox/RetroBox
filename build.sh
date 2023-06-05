#!/bin/sh
mkdir dist
dotnet publish RetroBox.Manager -r win-x64   -c Release --self-contained true -o dist/win
dotnet publish RetroBox.Manager -r linux-x64 -c Release --self-contained true -o dist/lin
dotnet publish RetroBox.Manager -r osx-x64   -c Release --self-contained true -o dist/osx
mkdir pub
zip -q -r pub/RetroBox.Manager_win.zip      dist/win
tar -czf  pub/RetroBox.Manager_linux.tar.gz dist/lin
tar -czf  pub/RetroBox.Manager_mac.tar.gz   dist/osx
