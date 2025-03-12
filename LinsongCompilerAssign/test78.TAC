Proc one

_bp-10 = 5
a = _bp-10

_bp-12 = 10
b = _bp-12

_bp-14 = a + b
@_bp+6 = _bp-14

_bp-16 =  - a
_bp-18 = 22 + _bp-16
_bp-4 = _bp-18
endp one
Proc eight

Push a

Push b
call one
endp eight
START eight
