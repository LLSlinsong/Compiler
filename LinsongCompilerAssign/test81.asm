       .MODEL SMALL
       .586
       .STACK 100H
       .DATA
a DW ?
b DW ?
d DW ?
_t1 DW ?
_t2 DW ?
_t3 DW ?
       .CODE
       INCLUDE IO.ASM
start PROC
       mov ax, @data
       mov ds,ax
       call one
       mov ah, 4ch
       mov al,0
       int 21h
start ENDP

one PROC
       PUSH BP
       MOV BP, SP
       SUB SP, 12


       mov ax, 5
       mov[_t1],ax

       mov ax, _t1
       mov[a],ax


       mov ax, 10
       mov[_t2],ax

       mov ax, _t2
       mov[b],ax


       mov ax, [a]
       mov bx, [b]
       imul bx
       mov [_t3], ax

       mov ax, _t3
       mov[d],ax

       mov dx,[d]
       call writeint

       call writeln

       ADD SP, 12
       POP BP
       RET 0

main PROC
       call one
main endp
end start

