Proc x
wrs _S0
rdi a

_bp-6 = 10
b = _bp-6

_bp-8 = 20
_bp-4 = _bp-8

_bp-10 = a * b
_bp-12 = _bp-4 + _bp-10
_bp-2 = _bp-12
wrs _S1
wri _bp-2
wrln
endp x
Proc two
call x
endp two
START two
