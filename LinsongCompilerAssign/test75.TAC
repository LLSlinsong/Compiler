Proc fun

_bp-4 = _bp+6 * _bp+4
_bp-2 = _bp-4
endp fun
Proc five

_t1 = 5
a = _t1

_t2 = 10
b = _t2

_t3 = 20
d = _t3

_t4 = a * b
_t5 = d + _t4
c = _t5

Push a

Push b
call fun
endp five
START five
