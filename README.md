# FleetWrencher Key Gen
## Developer: JT Networx
[Releases](https://github.com/JTNetworx/FleetWrencherKeyGenerator/releases "FleetWrecherKeyGen Releases")
## Interactive Base64 or Hex Key Generator With Security Scoring and Optional File Output

### Command Line Arguments
- _--length=_ The Key Length to generate (default: 32)
- _--format=_ The Format to geneate (default: Base64)
- _--output=_ The file output path (optional)
- _--help=_   The display of the command line arguments

### Supported Formats For Command Line Arguments
- Base64
- Hex
- Binary
- Base32
- Base58
- Base85
- URL-Safe-Base64

### Security Scoring
Score Name | Value
:---------: | :---: 
 *Very High* | `5` 
 *High*      | `4` 
 *Medium*    | `3` 
 *Low*       | `2` 
 *Very Low*  | `1` 


- _Key Length >= 24_                := +1
- _Key HasUpperCase_                := +1
- _Key Has LowerCase_               := +1
- _Key HasDigit_                    := +1
- _Key Has One Of ("!@#$%^&*()")_   = +1
