       .MODEL SMALL
       .586
       .STACK 100H
       .DATA
a DW ?
b DW ?
_S0 DB "Enter a number ", "$"
_S1 DB "The answer is ", "$"
       .CODE
       INCLUDE IO.ASM
start PROC
       mov ax, @data
       mov ds,ax
       call two
       mov ah, 4ch
       mov al,0
       int 21h
start ENDP

x PROC
       PUSH BP
       MOV BP, SP
       SUB SP, 12

       mov dx, offset_S0
       call writestr

       call readint
       mov [a], bx


       mov ax, 10
       mov[_bp-6],ax

       mov ax, _bp-6
       mov[b],ax


       mov ax, 20
       mov[_bp-8],ax

       mov ax, _bp-8
       mov[_bp-4],ax


       mov ax, [a]
       mov bx, [b]
       imul bx
       mov [_bp-10], ax

       mov ax, [_bp-4]
       add ax, [_bp-10]
       mov [_bp-12], ax

       mov ax, _bp-12
       mov[_bp-2],ax

       mov dx, offset_S1
       call writestr

       mov dx,[_bp-2]
       call writeint

       call writeln

       ADD SP, 12
       POP BP
       RET 0

two PROC
       PUSH BP
       MOV BP, SP
       SUB SP, 4

       call x

       ADD SP, 4
       POP BP
       RET 0

main PROC
       call two
main endp
end start

