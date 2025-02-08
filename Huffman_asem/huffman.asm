;Asembly Language - Project
;Krzyszkof Klecha
;section 11
;semester 5
;year 2024/25
;Huffman coding compresion
;version 1.1
;
;Dll library: asembly language

.code

;             MAKROES
;#--------------------------------#

;     Write tree node to array
;#--------------------------------#
makeNode MACRO nodeValAndByteXMM, pointersXMM
    lea RAX, [RDI+RCX*8]                ;calculate effective address of first node
    movdqu [RAX], nodeValAndByteXMM     ;push counter and bit to the output array
    add RCX, 2                          ;increase size of the array
    movdqu [RDI+RCX*8], pointersXMM     ;push pointers to the array
    add RCX, 2                          ;increase size of the array
ENDM

saveReg MACRO
    push RSP
    push RAX
    push RBX
    push RCX
    push RDX
    push RBP
    push R8
    push R9
    push R10
    push R11
    push R12
    push R13
    push R14
    push R15
    push RSI
    push RDI
ENDM

getReg MACRO
    pop RDI
    pop RSI
    pop R15
    pop R14
    pop R13
    pop R12
    pop R11
    pop R10
    pop R9
    pop R8
    pop RBP
    pop RDX
    pop RCX
    pop RBX
    pop RAX
    pop RSP
ENDM


; countBytes - counts every instance of bytes (0..255) to QWORD array from byte 
;              array where counterArray[byte] = counter
;
; Input:
;   RCX - pointer to source byte array 
;   RDX - length of the array 
;   R8  - pointer to destination counter array
;   (The array has to be QWORD[256])
;
; Output: 
;   Destinated array in RDI register filled with counters of each byte as a QWORD
;
;register usage:
;   RDI - begining of the bytes counting array
;   R9  - pointer to the array of files bytes
;   RBX - size of the file array - constans
;   RSI - file array iterator
;   RDX - file byte storage register
;
; Algorythm gets each byte from source array and increment QWORD value at index 
; of that byte.

countBytes proc stackPointer: QWORD

    saveReg                             ; Write regs on stack

    mov     stackPointer, RSP

;          register preparation
;#--------------------------------------#

	mov     RDI, R8                     ; Remember the adress where the bytes counting array begins
	mov     R9, RCX                     ; Remember the adress to file bytes array
	mov     RBX, RDX                    ; Move file array length to RBX
	xor     RSI, RSI                    ; Seting file array iterator to 0
	xor     RDX, RDX                    ; Cleaning register

;               main loop
;#--------------------------------------#

moveToNextFileByte:
	cmp     RSI, RBX                    ; Check if the file array did not end
	je      fileArrayEnd                ; If true stop searching else continue

	mov     DL, [R9 + RSI]              ; Read byte from file array
	inc     qword ptr [RDI + RDX*8]     ; Increase counter in the array

	add     RSI, 1                      ; Increase file array iterator
	jmp     moveToNextFileByte          ; Continue searching

;            end of procedure
;#--------------------------------------#

fileArrayEnd:
    mov     RSP, stackPointer
    
    getReg                              ; recover registers from stack 

	ret
countBytes endp

; makeSortedArray - creates a sorted array (ascending order) from an unordered byte counter array
; (where sourceArray[byte] = counter) into a new array of items (each item is a pair <byte, counter>)
; and excludes bytes with counter = 0.
;
; Input:
;   RCX - pointer to source counting array (each QWORD represents counter for byte index)
;   RDX - pointer to destination (sorted) array
;   (The source array is assumed to have 256 QWORDs; each index corresponds to a byte value 0..255)
;
;Output:
;   The destination array (pointed by RDX) is filled with items (QWORDs): first QWORD = byte value,
;   second QWORD = counter, etc. in ascending order by counter value;
;   For equal counters, the original order is preserved (stable sort).
;
; Register usage:
;   RDI - pointer to source counting array
;   RSI - pointer to destination sorted array
;   RCX - loop iterator for source array (0 to 255)
;   RBX - size (number of QWORDs written) in the sorted array
;   R10 - holds the current minimal counter value from the remaining items
;   RDX - used as minimal index (byte value with minimal counter) during search
;   R9  - number of iterations (number of items output so far)
;   RAX - general purpose temporary storage (loaded counter)
;
; The algorithm scans the source array (indices 0..255), and in each pass
; finds the minimal (non-zero) counter among the remaining entries.
; If two entries have the same counter value, the first (lower byte value) is chosen.
; Once an element is output, its counter is cleared to 0 so that it will not be re-selected.
;
; ################## WARNING - procedure destroyes input array ##################

makeSortedArray proc stackPointer: QWORD   ; stackPointer used to restore RSP later

    ; Save key registers on stack
    saveReg

    mov     stackPointer, RSP       ; Save current RSP for later restoration

    ; Register preparation
    xor     R9, R9                  ; Loop counter for output items = 0
    mov     RDI, RCX                ; RDI now points to source counting array
    mov     RSI, RDX                ; RSI now points to destination sorted array
    xor     RBX, RBX                ; Clear RBX; size of sorted array (each item uses 2 QWORDs)
    
;            Main loop
;         Reset registers
;-----------------------------------;
MainLoop:
    xor     RCX, RCX                ; RCX = 0, iterate over index 0..255
    mov     RDX, -1                 ; RDX will hold the minimal index (byte) found in current pass; -1 means none found
    xor     R10, R10
    not     R10                     ; Reset minimal counter to maximum possible

;          Find minimum
;-----------------------------------;
FindMinLoop:
    cmp     RCX, 256                ; If we've reached the end of the source array
    je      ProcessMin              ; then process the found minimal element (if any)
    
    mov     RAX, [RDI + RCX*8]      ; RAX = counter for byte RCX
    cmp     RAX, 0
    je      NextIndex               ; If counter is 0, skip this index

    cmp     RDX, -1                 ; If no minimal index set yet
    je      SetMin                  ; Then set current index as minimum
    
    cmp     RAX, R10                ; Compare with current minimal counter in R10
    jb      SetMin                  ; If new counter < current minimum, update minimal index
                                    ; For equal counters, do not update, so the earlier remains
    jmp     NextIndex

SetMin:
    mov     RDX, RCX                ; Set minimal index to RCX
    mov     R10, RAX                ; Set minimal counter value to RAX

NextIndex:
    inc     RCX
    jmp     FindMinLoop

;           End of loop
;-----------------------------------;
ProcessMin:
    cmp     RDX, -1                 ; If no element was found in this pass
    je      EndLoop                 ; Then end procedure

;  Write counter and byte in output 
;             array
;-----------------------------------;
    mov     [RSI + RBX*8], RDX      ; Save the byte value (the index) in sorted array
    inc     RBX                     ; Increase size counter
    mov     RAX, [RDI + RDX*8]      ; Load the counter for that byte
    mov     [RSI + RBX*8], RAX      ; Save the counter
    inc     RBX                     ; Increase size counter by 2 QWORDs per item

    mov     qword ptr[RDI+RDX*8], 0 ; Mark the element as processed (set counter to 0) so it will not be chosen again

    inc     R9                      ; Increase loop iteration count

    
    cmp     R9, 256                 ; Check if we have processed all 256 possible bytes
    jl      MainLoop                ; If not, continue searching

EndLoop:

    ; Restore registers and return
    getReg

    ret

makeSortedArray endp


; makeHuffmanCodeTree - create binary tree of Huffmans code. Gets sorted array of 
;                      pairs<byte, counter> and merge each pair creating treeNodes
;
;   struct treeNode{
;       QWORD   byte
;       QWORD   weight
;       QWORD   leftPointer
;       QWORD   rightPointer
;   }
;
; Input: 
;   RCX - pointer to the sorted array of pairs<byte, counter>
;   RDX - size of the input array
;   R8  - pointer to the array which should be filled with the nodes
;
; Output: 
;   Array of treeNodes where at the begining stands the node with lesed weight.
;   Weights of nodes are growing and the last node in the array is the root.
;   Order of the bytes is preserved.
;
; Register usage:
;   RAX  - for pointer storage
;   RSI  - pointer to input array
;   RBX  - input array iterator
;   RDX  - size of the input array
;   RDI  - output array pointer
;   R9   - orphan node counter
;   R10  - iterator for search where to insert node in the input array
;   xmm0 - node one
;   xmm1 - pointers of node one
;   xmm2 - node two
;   xmm3 - pointers of node two
;   xmm4 - loaded node from orphan nodes array
;   xmm5 - for shifting elements in array
;
; Algorythm read two pair of byteCounters, checks if bytes is in range 0..255. 
; If it's so then creates treeNodes in destination array and creates an orphan node
; in source array. Orphan node has counter just like countByte but has pointer to array of 
; pointers at stack. This array holds pointers to previously created nodes in output array.
; Orphan node is placed at before first greater or equal index in the array. Rest items are 
; shifted, so array stays sorted. In next iterations orphan nodes are recognized and loaded 
; to registers with it's pointers.
;
; ################## WARNING - procedure destroyes input array ##################
makeHuffmanCodeTree proc stackPointer: QWORD

;       save key register on stack
;#-------------------------------------#

    saveReg

    mov stackPointer, RSP

;         register preparation
;#-------------------------------------#

    mov RSI, RCX                        ; Keep the pointer to input array in RSI register
    xor RBX, RBX                        ; Clean register
    mov RDI, R8                         ; Keep pointer to output array in RDI
    xor RCX, RCX                        ; Clean register
	xor R8, R8                          ; Clean register

;              main loop
;---------------------------------------;
start:
    cmp RBX, RDX                        ; Check end of array
    jge endCreatingTree

    pxor xmm1, xmm1
    pxor xmm3, xmm3

    movdqu xmm0, [RSI+RBX*8]            ; Load byte and counter
    add RBX, 2                          ; Increment input array iterator
    
    movdqu xmm2, [RSI+RBX*8]            ; Load next byte and counter

    pextrq RAX, xmm0, 0                 ; Get byte
    cmp RAX, 255                        ; And check if it is a byte
    jg notAByte1

checkSecondNode:
    pextrq RAX, xmm2, 0                 ; Get byte
    cmp RAX, 255                        ; And check if it is a byte
    jg notAByte2
    jmp createNodes

notAByte1:
     movdqu xmm1, [RAX]                 ; Load pointers of the node
     xor RAX, RAX
     pinsrq xmm0, RAX, 0
     jmp checkSecondNode                ; Check other node

notAByte2:
    movdqu xmm3, [RAX]                  ; Load pointers of the node
    xor RAX, RAX
    pinsrq xmm2, RAX, 0

;         Node creation stage
;--------------------------------------;
createNodes:
    makeNode xmm0, xmm1
    movq xmm4, RAX                      

    makeNode xmm2, xmm3
    pinsrq xmm4, RAX, 1

    paddq xmm0, xmm2                    ;calcualte weight of the node
    sub RSP, 16                         ;make space for write pointers of the orphan node

    movdqu [RSP], xmm4                  ;write pointers on stack
    pinsrq xmm0, RSP, 0                 ;save pointer on orphan nodes pointers

;      write orphan node to array
;--------------------------------------;
    pextrq R9, xmm0, 1                 ;get orphan node counter 
    

    mov R10, RBX                        ;set reg
searchInsert:
    mov RAX, [RSI + R10*8+8]            ;get counter
    cmp RAX, R9                         ;if node counter is greater then current counter
    jge writeNode
    cmp R10, RDX
    je writeNode
    add R10, 2
    jmp searchInsert

writeNode:
    sub R10, 2
    movdqu xmm2, [RSI+R10*8]             ;get byteCounter
    movdqu [RSI+R10*8], xmm0             ;write orphan node


shiftRigthInArray:
    sub R10, 2                           ;decrement iterator to previous value
    cmp R10, 0                           ;if iterator is equal 0
    je start                             ;then go to next elements in input array
    movdqu xmm5, [RSI+R10*8]             ;read previous value
    movdqu [RSI+R10*8], xmm2             ;write current value
    movdqu xmm2, xmm5                    ;change value to write in next loop
    jmp shiftRigthInArray


;           load key register
;#-------------------------------------#
endCreatingTree:
    mov RSP, stackPointer
    
    getReg

    ret                                 ;end

makeHuffmanCodeTree endp

; codeFile - code input byte array by code writed in char** array, where at each index stays 
;            code in ASCII format ('0', '1') for byte that index represents. char array has to be
;            null terminated.
;
; Input:
;   RCX - pointer to the code table (char**)
;   RDX - pointer to input byte array (file data)
;   R8  - pointer to output byte array (coded output bitstream)
;   R9  - size of input array (number of bytes)
;
;Output:
; The procedure reads each input byte, obtains its corresponding code string (from the code table),
; then writes the bits (0s and 1s) consecutively into the output array.
;
; Registers used:
;   RCX - use for shifting
;   RDX - input array pointer
;   R8  - output array pointer
;   R9  - size of input array
;   R12 - input index (i) (0 .. R9-1)
;   R10 - current output byte (bit buffer)
;   R11 - current bit index (0 to 7)
;   RBX - temporary register (used for obtaining code pointer)
;   R14 - code table pointer
;   R15 - temporary register for bit position computation
;
; Algorythm loads byte from source array reads code of that byte (in ASCII)
; and creates byte based on that code
;
codeFile1 proc
    
    saveReg

    xor     R12, R12                ; input index, initialize to 0
    xor     R10, R10                ; current output byte, initialize to 0
    xor     R11, R11                ; bit index in output byte, initialize to 0
    xor     RAX, RAX        

    mov     R14, RCX

;    Get pointer to code string
;-----------------------------------;
encodeLoop:
    cmp     R12, R9
    jae     flushOutput             ; If input index >= input size then flush output

    mov   AL, byte ptr [RDX + R12]  ; Load next input byte from input array
    
    mov     RBX, RAX                ; Each pointer is 8 bytes, so multiply index by 8
    shl     RBX, 3                  ; input byte * 8
    
    mov     RBX, qword ptr [R14+RBX]; Get the pointer to the code string from the code table

;     Process the code string
;-----------------------------------;
codeLoop:
    mov     AL, byte ptr [RBX]      ; load next character from code string
    cmp     AL, 0
    je      nextInput               ; If null terminator then move to next input byte
    
    sub     AL, '0'                 ; Convert ASCII '0'/'1' to bit value:

    ; Compute bit position in current output byte.
    ; We choose to pack bits from most significant bit downward.
    ; Bit position = 7 - R11.

    mov     R15, 7
    sub     R15, R11                ; R15 = 7 - current bit index
    
    cmp     AL, 0                   ; If bit is 1 then set that bit in r10.
    je      skipSetBit
    mov     CL, R15b                ; Set position for shifting
    mov     RAX, 1
    shl     RAX, CL                 ; RAX = 1 << (7 - R11)
    or      R10, RAX                ; Set that bit in the output byte
skipSetBit:
    inc     R11                     ; increment bit index in current output byte

;  If byte is filled with code then
;        write it to memory
;-----------------------------------;
    cmp     R11, 8                  ; If we filled 8 bits 
    jne     continueCode            ; Then write out the output byte
    mov     byte ptr [R8], R10b     ; store r10 into output buffer
    inc     R8                      ; Move output pointer to next byte
    xor     R10, R10                ; Clear output byte
    xor     R11, R11                ; Reset bit index

continueCode:
    add     RBX, 2                     ; Move to next character in code string
    jmp     codeLoop

nextInput:
    inc     R12                     ; move to next input byte
    jmp     encodeLoop

;    Write to memory rest of bits
;-----------------------------------;

flushOutput:
    
    cmp     R11, 0                  ; If there is a partially filled output byte
    je      endProc                 ; then flush it
    mov     byte ptr [R8], R10b


; Restore registers and return
;-----------------------------------;
endProc:

    getReg
    ret

codeFile1 endp

end