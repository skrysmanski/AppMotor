#region License
// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using AppMotor.Core.Certificates.Pem;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates.Pem;

public sealed class PemFileInfoTests
{
    [Fact]
    public void TestSingleBlockFile()
    {
        const string CONTENTS = @"
-----BEGIN CERTIFICATE-----
MIIFwTCCA6mgAwIBAgIUWDxRxdUBMGqfq+dtYn8zV2jF5bowDQYJKoZIhvcNAQEL
BQAwcDELMAkGA1UEBhMCVVMxDzANBgNVBAgMBk9yZWdvbjERMA8GA1UEBwwIUG9y
-----END CERTIFICATE-----
";

        var fileInfo = new PemFileInfo(CONTENTS);

        fileInfo.Blocks.Length.ShouldBe(1);
        fileInfo.Blocks[0].BlockType.ShouldBe("CERTIFICATE");
        CONTENTS[fileInfo.Blocks[0].BlockContentRange].ShouldStartWith("MIIFwTCC", Case.Sensitive);
        CONTENTS[fileInfo.Blocks[0].BlockContentRange].ShouldEndWith("EBwwIUG9y", Case.Sensitive);
    }

    [Fact]
    public void TestMultiBlockFile()
    {
        const string CONTENTS = @"
-----BEGIN CERTIFICATE1-----
MIIFwTCCA6mgAwIBAgIUWDxRxdUBMGqfq+dtYn8zV2jF5bowDQYJKoZIhvcNAQEL
BQAwcDELMAkGA1UEBhMCVVMxDzANBgNVBAgMBk9yZWdvbjERMA8GA1UEBwwIUG9y
-----END CERTIFICATE1-----

-----BEGIN CERTIFICATE2-----
X+SSL1oeUHZM2lBcLAdrg/yZ9L8SpOhWdM22O7Vadw4dTOoGWUk4/l2bNFQpvLro
niBD+MrOEirPPEPhpNo0ElfcVG31dx3mmqujArl0g/at3UngVrUHAgMBAAGjUzBR
-----END CERTIFICATE2-----
";

        var fileInfo = new PemFileInfo(CONTENTS);

        fileInfo.Blocks.Length.ShouldBe(2);

        fileInfo.Blocks[0].BlockType.ShouldBe("CERTIFICATE1");
        CONTENTS[fileInfo.Blocks[0].BlockContentRange].ShouldStartWith("MIIFwTCC", Case.Sensitive);
        CONTENTS[fileInfo.Blocks[0].BlockContentRange].ShouldEndWith("EBwwIUG9y", Case.Sensitive);

        fileInfo.Blocks[1].BlockType.ShouldBe("CERTIFICATE2");
        CONTENTS[fileInfo.Blocks[1].BlockContentRange].ShouldStartWith("X+SSL1oe", Case.Sensitive);
        CONTENTS[fileInfo.Blocks[1].BlockContentRange].ShouldEndWith("AAGjUzBR", Case.Sensitive);
    }

    [Fact]
    public void TestMalformedStart()
    {
        const string CONTENTS = @"
-----ABC CERTIFICATE-----
MIIFwTCCA6mgAwIBAgIUWDxRxdUBMGqfq+dtYn8zV2jF5bowDQYJKoZIhvcNAQEL
";

        var ex = Should.Throw<PemFormatException>(() => new PemFileInfo(CONTENTS));
        ex.Message.ShouldBe("Malformed PEM file (expected block begin).");
    }

    [Fact]
    public void TestMalformedEnd()
    {
        const string CONTENTS = @"
-----BEGIN CERTIFICATE-----
MIIFwTCCA6mgAwIBAgIUWDxRxdUBMGqfq+dtYn8zV2jF5bowDQYJKoZIhvcNAQEL
-----ABC CERTIFICATE-----
";

        var ex = Should.Throw<PemFormatException>(() => new PemFileInfo(CONTENTS));
        ex.Message.ShouldBe("Malformed PEM file (expected block end).");
    }

    [Fact]
    public void TestMismatchedBlockTypes()
    {
        const string CONTENTS = @"
-----BEGIN CERTIFICATE1-----
MIIFwTCCA6mgAwIBAgIUWDxRxdUBMGqfq+dtYn8zV2jF5bowDQYJKoZIhvcNAQEL
-----END CERTIFICATE2-----
";

        var ex = Should.Throw<PemFormatException>(() => new PemFileInfo(CONTENTS));
        ex.Message.ShouldBe("Malformed PEM file (block types don't match).");
    }

    [Fact]
    public void TestEmptyBlock()
    {
        const string CONTENTS = @"
-----BEGIN CERTIFICATE-----
-----END CERTIFICATE-----
";

        var ex = Should.Throw<PemFormatException>(() => new PemFileInfo(CONTENTS));
        ex.Message.ShouldBe("Malformed PEM file (no block contents).");
    }

    [Fact]
    public void TestMissingBegin_SingleBlock()
    {
        const string CONTENTS = @"
MIIFwTCCA6mgAwIBAgIUWDxRxdUBMGqfq+dtYn8zV2jF5bowDQYJKoZIhvcNAQEL
";

        var ex = Should.Throw<PemFormatException>(() => new PemFileInfo(CONTENTS));
        ex.Message.ShouldBe("Malformed PEM file (missing block begin).");
    }

    [Fact]
    public void TestMissingBegin_MultipleBlock()
    {
        const string CONTENTS = @"
-----BEGIN CERTIFICATE-----
MIIFwTCCA6mgAwIBAgIUWDxRxdUBMGqfq+dtYn8zV2jF5bowDQYJKoZIhvcNAQEL
-----END CERTIFICATE-----

MIIFwTCCA6mgAwIBAgIUWDxRxdUBMGq
";

        var ex = Should.Throw<PemFormatException>(() => new PemFileInfo(CONTENTS));
        ex.Message.ShouldBe("Malformed PEM file (missing block begin).");
    }

    [Fact]
    public void TestEmptyFile()
    {
        const string CONTENTS = "";

        var ex = Should.Throw<PemFormatException>(() => new PemFileInfo(CONTENTS));
        ex.Message.ShouldBe("Malformed PEM file (no content).");
    }
}