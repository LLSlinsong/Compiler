       .MODEL SMALL
       .586
       .STACK 100H
       .DATA
a DW ?
b DW ?
d DW ?
_S0 DB "The answer is ", "$"
_t3 DW ?
_t4 DW ?
_t5 DW ?
       .CODE
       INCLUDE IO.ASM
start PROC
       mov ax, @data
       mov ds,ax
       call three
       mov ah, 4ch
       mov al,0
       int 21h
start ENDP

fun PROC
       PUSH BP
       MOV BP, SP
       SUB SP, 6


       mov ax, [_bp+8]
       mov bx, [_bp+6]
       imul bx
       mov [_bp-4], ax

       mov ax, [_bp+4]
       add ax, [_bp-4]
       mov [_bp-6], ax

       mov ax, _bp-6
       mov[_bp-2],ax

       mov dx, offset_S0
       call writestr

       mov dx,[_bp-2]
       call writeint

       call writeln

       ADD SP, 6
       POP BP
       RET 0

three PROC
       PUSH BP
       MOV BP, SP
       SUB SP, 12


       mov ax, 5
       mov[_t3],ax

       mov ax, _t3
       mov[a],ax


       mov ax, 10
       mov[_t4],ax

       mov ax, _t4
       mov[b],ax


       mov ax, 20
       mov[_t5],ax

       mov ax, _t5
       mov[d],ax


       push b


       push d


       push a

       call fun

       ADD SP, 12
       POP BP
       RET 0

main PROC
       call three
main endp
end start

