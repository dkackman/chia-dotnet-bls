# blst

This uses the [blst](https://github.com/supranational/blst) project.

## blst builds

### win-x64

On a windows machine with the Visual Studio 2019 or 2022 build tools installed, run the following command form teh developer tools command line to build the blst library.


```powershell
.\build.bat -shared -dll
```

### linux-x64

On a linux machine, with `build-essential` installed, run the following command to build the blst library.

```bash
./build.sh -shared -dll
```

### linux-arm64

On a linux machine, with `build-essential` and the cross compilation tartget for aarch64 (`apt install g++-10-aarch64-linux-gnu libstdc++-10-dev-arm64-cross gcc-10 g++-10`) installed, run the following command to build the blst library.

```bash
./build.sh -shared -dll CC=aarch64-linux-gnu-gcc-10 flavor=elf
```

### macos-x64

TBD