Proc fun

_bp-4 = _bp+8 * _bp+6
_bp-6 = _bp+4 + _bp-4
_bp-2 = _bp-6
wrs _S0
wri _bp-2
wrln
endp fun
Proc three

_t3 = 5
a = _t3

_t4 = 10
b = _t4

_t5 = 20
d = _t5

Push b

Push d

Push a
call fun
endp three
START three
