# FleetWrencher Key Gen
## Developer: JT Networx

## Interactive Base64 or Hex Key Generator With Security Scoring and Optional File Output

### Command Line Arguments
_--length=_ The Key Length to generate (default: 32)
_--format=_ The Format to geneate (default: Base64 or Hex)
_--output=_ The file output path (optional)
_--help=_   The display of the command line arguments

### Security Scoring
_"Very High"_    Score Value:5
_"High"_         Score Value:4
_"Medium"_       Score Value:3
_"Low"_          Score Value:2
_"Very Low"_     Score Vlaue:1

_Key Length >= 24_                = +1
_Key HasUpperCase_                = +1
_Key Has LowerCase_               = +1
_Key HasDigit_                    = +1
_Key Has One Of ("!@#$%^&*()")_   = +1
