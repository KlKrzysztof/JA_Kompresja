;Asembly Language - Project
;Krzyszkof Klecha
;section 11
;semester 5
;year 2024/25
;Huffman coding compresion
;version 0.1
;
;Dll library: asembly language
.code

;countBytes - count bytes from one array and fill second one with bytes counters
;input: pointer to the array where we want to count bytes in RCX; length of the array in RDX; pointer to 256 element array of QWORDs in R8
;output: array from register R8 filled with counters
;
;register usage:
;RDI - begining of the bytes counting array
;R9 - pointer to the array of files bytes
;RAX - mixed usage
;RBX - size of the file array - constans
;ECX - for stosd usage
;RSI - file array iterator
;RDX - file byte storage register
countBytes proc 

;   register preparation
;#---------------------------------#

	mov RDI, R8                     ;remember the adress where the bytes counting array begins
	mov R9, RCX                     ;remember the adress to file bytes array
	mov RBX, RDX                    ;move file array length to RBX
	xor RSI, RSI                    ;seting file array iterator to 0
	xor RAX, RAX                    ;set 0 as value which will be written to the array 
	xor RDX, RDX                    ;cleaning register

;   main loop
;#---------------------------------#

moveToNextFileByte:
	cmp RSI, RBX                    ;check if the file array did not end
	je fileArrayEnd                 ;if true stop searching else continue

	mov DL, [R9 + RSI]              ;read byte from file array
	inc qword ptr [RDI + RDX*8]     ;increase counter in the array
	mov RAX, [RDI + RDX*8]          ;for debug purpose only;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

	add RSI, 1                      ;increase file array iterator
	jmp moveToNextFileByte          ;continue searching

;   end of procedure
;#---------------------------------#

fileArrayEnd:
	ret
countBytes endp

;COMPLETED, NOT TESTED PROPERLY
;makeSortedArray - makes from unordered bytes counter array (where array[byte] = counter) new array in growing order 
;(where an item is ($byte|$counter)) without bytes with counter = 0
;input: pointer to source array in RCX; pointer to destinate array in RDX
;output: modified array from RDX register
;
;register usage:
;RDI - pointer to counter array
;RSI - sorted array pointer
;RCX - counter array iterator
;RBX - size of sorted array
;R10 - previosu scope minimal counter
;RDX - index of minimal counter
;R8 - sorted array iterator
makeSortedArray proc loopCounter:BYTE   ;variable counting loops over counting table (0 - 255)

;   register preparation
;#-------------------------------------#

    mov loopCounter, 0                  ;set loops to 0
    mov RDI, RCX                        ;keep pointer to counting array
    mov RSI, RDX                        ;keep sorting array pointer

    xor RCX, RCX                        ;clean register
    xor RBX, RBX                        ;clean register
    xor R10, R10                        ;clean register

;   main loop
;#-------------------------------------#

searchForMin:
	mov RDX, -1                         ;set information that minimal index is not set
    xor RAX, RAX                        ;clean register

findMinLoop:
    cmp RCX, 256                        ;if the counting array ends
    je endOfArray                       ;then write the minimum, else continue searching

    mov RAX, [RDI+RCX*8]                ;load counter from array
    cmp RAX, 0                          ;if is zero
    je skipToNext                       ;then skip this value, else continue

    cmp RAX, R10                        ;compare with previous minimum
    jl skipToNext                       ;if is smaller skip it
	je searchInWritedBytes              ;if is equal then check if the byte is already included in the sorted array

byteNotIncluded:
    cmp RDX, -1                         ;if minimal index was setted
    jne compareWithCurrentMin           ;than compare with current minimum
    mov RDX, RCX                        ;else set first index
    jmp skipToNext                      ;move to next value

compareWithCurrentMin:
    mov R9, [RDI+RDX*8]                 ;load minimal counter
    cmp RAX, R9                         ;compare with current counter
    jge skipToNext                      ;if new value is greater than skip
    mov RDX, RCX                        ;else update the index

skipToNext:
    inc RCX                             ;increase the counting array iterator
    jmp findMinLoop                     ;continue searching

;searching if byte was already included 
;#-------------------------------------#

searchInWritedBytes:
	xor R8, R8                          ;clean register
searchLoop:
	rol R8, 4                           ;multiply iterator by 16
	mov RAX, [RSI+R8]                   ;load byte from sorted array
	cmp RAX, RCX                        ;check if the byte was already included in the array
	je skipToNext                       ;then skip, else continue
	ror R8, 4                           ;divide iterator by 16
	inc R8                              ;move to next byte
	cmp R8, RBX                         ;check end of the array
	je byteNotIncluded                  ;if true byte is not included in the sorted array
	jmp searchLoop                      ;else continue search loop

;searching ends - save minimal byte and 
;     counter in the sorted array
;#-------------------------------------#

endOfArray:
    mov [RSI+RBX*8], RDX                ;save the byte
    mov RAX, [RDI+RDX*8]                ;load its counter
    inc RBX                             ;increment the size of the array
    mov [RSI+RBX*8], RAX                ;save a counter
    inc RBX                             ;increment the size of the array

    mov R10, RAX                        ;set previous counter as current one

    xor RCX, RCX                        ;reset iterator
	xor AX, AX                          ;clean register
    mov AL, loopCounter                 ;load loop counter
    inc AX                              ;increase counter
    mov loopCounter, AL                 ;save to variable
    cmp AX, 255                         ;check if all array was searched
    jb searchForMin                     ;if not search again

    ret
makeSortedArray endp

;------Dodaæ sprawdzanie rozmiarów tablic
;makeHuffmanCodeTree - create binary tree of Huffmans code from table of bytes and theirs counters. Each cell is a QWORD
;input: pointer to the array of QWORDs where the structure is ($byte|$counter), size of the array, pointer to the array which should be filled with the tree
;output: Modified array from 3rd param
;
;register usage:
;RSI - pointer to input array
;RBX - input array iterator
;RDX - size of the input array
;RDI - output array pointer
;RCX - size of output array
;xmm0 - node one
;xmm1 - pointers of node one
;xmm2 - node two
;xmm3 - pointers of node two
;xmm4 - loaded node from orphan nodes array
;xmm5 & xmm6 - for comparison puropse only
;R8 - iterator of the orphan nodes array
;R9 - size of the orphan nodes array
;R10 - use for temporary storage
;R11 - use for temporary storage
makeHuffmanCodeTree proc stackPointer: QWORD

;       save key register on stack
;#-------------------------------------#

    push RSP
    push RBX
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

    mov stackPointer, RSP

;         register preparation
;#-------------------------------------#

    mov RSI, RCX                        ;keep the pointer to input array in RSI register
    xor RBX, RBX                        ;clean register
    mov RDI, R8                         ;keep pointer to output array in RDI
    xor RCX, RCX                        ;clean register
	xor R8, R8                          ;clean register
    xor R9, R9                          ;clean register
	mov R11, RDI                        ;set root pointer as a refference point

;start of state machine deciding how to 
;    combine nodes in current step 
;To do:
;   trzeba uwzglêdniæ, ¿e mo¿e nie byæ 
;   wartoœci aby pobraæ wierwszy wêze³
;#-------------------------------------#

stateOne:                               ;state 1: we take two values from input array 
    movdqu xmm0, [RSI+RBX*8]            ;load byte and counter
    add RBX, 2                          ;increment input array iterator
    cmp RBX, RDX
    jne continueStateOne                ;if in array are still values than load one
    movdqu xmm2, [RSP+R8*8]             ;else load  from orphan nodes
    jmp continueStateTwo                ;and to to state 2
continueStateOne:
    movdqu xmm2, [RSI+RBX*8]            ;load next byte and counter
    cmp R9, R8                          ;check if orphan nodes array ends
    je endOfStateOne                    ;true: create 2 nodes and connect it by parent node
searchForValues:
    movdqu xmm4, [RSP+R8*8]             ;load orphan node
    pshufd xmm5, xmm4, 01001101b        ;change order of data from orphan node
    pshufd xmm6, xmm2, 01001101b        ;change order of data from node 2
    pcmpgtq xmm6, xmm5                  ;check if orphan node is less than greater value from input array
    movq RAX, xmm6                      ;load mask to register
    test RAX, RAX                       ;check result of comparison
    jz goToStateTwo                     ;if result is "less than" than go to state 2 
    add R8, 4                           ;else move iterator to next orphan node
    jmp searchForValues                 ;continue searching

; State 2: One orphan node is less than 
;   greater value in nodes from input 
;                 array
;#-------------------------------------#

stateTwo:
    cmp R9, R8                          ;check if orphan nodes array ends
    je endOfStateTwo                    ;true: create 1 node and connect it with orphan by parent node
    movdqu xmm4, [RSP+R8*8]             ;load orphan node
    pshufd xmm5, xmm4, 01001101b
    movq RAX, xmm5                      ;load weight of the node to RAX
    test RAX, RAX                       ;check if node is not null
    jnz continueStateTwo                ;if false: jump over procedure of starting this state
    add R8, 4                           ;else jump to next node (it means that its deleted node)
    jmp stateTwo                        ;start checking next node
goToStateTwo:
    movdqu xmm2, xmm4                   ;load to node 2 an orphan node 
    lea R10, [RSP+R8*8]                 ;keep pointer to an orphan node 
    movdqu xmm3, [R10+16]               ;load pointers of an orphan node
continueStateTwo:
    pshufd xmm6, xmm0, 01001101b        ;change order of data from node 1
    pcmpgtq xmm6, xmm5                  ;check if an orphan node is less than less value from input array
    movq RAX, xmm6                      ;load mask to register
    test RAX, RAX                       ;check result of comparison
    jz goToStateThree                   ;if result is "less than" than go to state 2 

    pshufd xmm6, xmm2, 01001101b        ;change order of data from node 2
    pcmpgtq xmm6, xmm5                  ;check if an orphan node is less than current orphan node
    movq RAX, xmm6                      ;load mask to register
    test RAX, RAX                       ;check result of comparison
    jz loadNewNode                      ;if node is less than current than change node

    add R8, 4                           ;else move iterator to next orphan node
    jmp stateTwo

loadNewNode:
    movdqu xmm2, xmm4                   ;load new node as a current one
    add R8, 4                           ;move the iterator
    lea R10, [RSP+R8*8]                 ;keep pointer to an orphan node 
    movdqu xmm3, [R10+16]               ;load pointers of an orphan node
    jmp stateTwo

;State 3: One orphan node is less than 
;         all value in new nodes
;#-------------------------------------#

goToStateThree:
    lea R11, [RSP+R8*8]                 ;keep pointer to another orphan node
    cmp R11, R10                        ;check if pointers are equal
    je stateThree                       ;if true than start state 3
    jmp goToStateFive                   ;else it is two value that should be taken from orphan nodes
stateThree: 
    cmp R9, R8                          ;check if orphan nodes array ends
    je endOfStateFour                   ;true: create 1 node and connect it with orphan by parent node
    movdqu xmm4, [RSP+R8*8]             ;load orphan node
    add R8, 4                           ;increase the iterator of orphan nodes array
    pshufd xmm5, xmm4, 01001101b        ;change order of data from orphan node
    pshufd xmm6, xmm0, 01001101b        ;change order of data from node 1

    pcmpgtq xmm6, xmm5                  ;check if orphan node is less than less value from input array
    movq RAX, xmm6                      ;load mask to register
    test RAX, RAX                       ;check result of comparison
    jz goToStateFive                    ;if true than exists nodes which are less than values from input array, so find 2 minimal nodes
    jmp stateThree                      ;else continue searching

;  State 5: Two orphan nodes are less 
;       than values in new nodes
;#-------------------------------------#

goToStateFive:
    movdqu xmm0, xmm4                   ;set node 1 as an orphan one
    movdqu xmm1, [R11+16]               ;load pointer of an orphan node
stateFive:                              
    cmp R9, R8                          ;check if orphan nodes array ends
    je endOfStateFive                   ;true: connect orphan nodes by parent node
    movdqu xmm4, [RSP+R8*8]             ;load orphan node
    add R8, 4                           ;increase the iterator of orphan nodes array

    pshufd xmm5, xmm2, 01001101b        ;change roder of data from node 2
    pshufd xmm6, xmm0, 01001101b        ;change order of data from node 1
    pcmpgtq xmm6, xmm5                  ;check which node is greater
    movq RAX, xmm6                      ;storage result of comparison

    pshufd xmm5, xmm4, 01001101b        ;change order of data from orphan node

    test RAX, RAX
    jnz xmm0Greater
    pshufd xmm6, xmm2, 01001101b        ;change order of data from node 1
    pcmpgtq xmm6, xmm5                  ;check if orphan node is less than node 1
    movq RAX, xmm6                      ;load mask to register
    test RAX, RAX                       ;check result of comparison
    jnz stateFive
    movdqu xmm2, xmm4
    jmp stateFive

xmm0Greater:
    pshufd xmm6, xmm0, 01001101b        ;change order of data from node 1
    pcmpgtq xmm6, xmm5                  ;check if orphan node is less than node 1
    movq RAX, xmm6                      ;load mask to register
    test RAX, RAX                       ;check result of comparison
    jnz stateFive
    movdqu xmm0, xmm4
    jmp stateFive

;             Ends of state
;#-------------------------------------#

;     State 1: save two new nodes
;#-------------------------------------#

endOfStateOne:

    ;set nullptrs in leaves
    pxor xmm1, xmm1
    pxor xmm3, xmm3

    ;Node 1
    lea RAX, [RDI+RCX*8]                ;calculate effective address of first node
    movdqu [RAX], xmm0                  ;push counter and bit to the output array
    add RCX, 2                          ;increase size of the array
    movdqu [RDI+RCX*8], xmm1            ;push pointers to the array
    add RCX, 2                          ;increase size of the array
    movq xmm1, RAX                      ;load address to lower part of the register

    ;Node 2
    lea RAX, [RDI+RCX*8]                ;calculate effective address of second node
    movdqu [RAX], xmm0                  ;push counter and bit to the output array
    add RCX, 2                          ;increase size of the array
    movdqu [RDI+RCX*8], xmm1            ;push pointers to the array
    add RCX, 2                          ;increase size of the array
    pinsrq xmm1, RAX, 1                 ;load address to upper part of the register

    ;create an orphan node
    paddq xmm0, xmm2                    ;calcualte weight of the node
    xor RAX, RAX                        ;set 0 in RAX
    movq xmm0, RAX                      ;zero byte
    sub RSP, 16                         ;move stack pointer
    movups [RSP], xmm1                  ;save nodes pointers
    sub RSP, 16                         ;move stack pointer
    movups [RSP], xmm0                  ;save weight of the node

    
    inc R9                              ;increase size of the orphan node array
    cmp RDX, RBX                        ;check end of input array
    xor R8, R8                          ;set iterator to 0
    jne StateOne
    jmp endOfProc

;State 2 and 4: save one new node and 
;            an orphan one
;#-------------------------------------#

endOfStateTwo:
endOfStateFour:
    ;set nullptrs in leave
    pxor xmm1, xmm1

    ;Node 1
    lea RAX, [RDI+RCX*8]                ;calculate effective address of first node
    movdqu [RAX], xmm0                  ;push counter and bit to the output array
    add RCX, 2                          ;increase size of the array
    movdqu [RDI+RCX*8], xmm1            ;push pointers to the array
    add RCX, 2                          ;increase size of the array
    movq xmm1, RAX                      ;load address to lower part of the register

    ;Write an orphan node to the output array
    lea RAX, [RDI+RCX*8]                ;calculate effective address of first node
    movdqu [RAX], xmm2                  ;push counter and bit to the output array
    add RCX, 2                          ;increase size of the array
    movdqu [RDI+RCX*8], xmm3            ;push pointers to the array
    add RCX, 2                          ;increase size of the array
    pinsrq xmm1, RAX, 1                 ;load address to upper part of the register

    ;create an orphan node
    paddq xmm0, xmm2                    ;calcualte weight of the node
    xor RAX, RAX                        ;set 0 in RAX
    movq xmm0, RAX                      ;zero byte
    sub RSP, 16                         ;move stack pointer
    movups [RSP], xmm1                  ;save nodes pointers
    sub RSP, 16                         ;move stack pointer
    movups [RSP], xmm0                  ;save weight of the node

    ;delete used orphan node from array
    pxor xmm2, xmm2
    movdqu [R10], xmm2 
    movdqu [R10+16], xmm2 

    dec RBX                             ;one value from array was taken
    inc R9                              ;increase size of the orphan node array
    cmp RDX, RBX                        ;check end of input array
    xor R8, R8;                         set iterator to 0
    jne StateOne
    jmp endOfProc

;    State 5: save two orphan nodes
;#-------------------------------------#

endOfStateFive:
    ;Write an orphan node 1 to the output array
    lea RAX, [RDI+RCX*8]                ;calculate effective address of first node
    movdqu [RAX], xmm0                  ;push counter and bit to the output array
    add RCX, 2                          ;increase size of the array
    movdqu [RDI+RCX*8], xmm1            ;push pointers to the array
    add RCX, 2                          ;increase size of the array
    movq xmm1, RAX                      ;load address to lower part of the register

    ;Write an orphan node 2 to the output array
    lea RAX, [RDI+RCX*8]                ;calculate effective address of first node
    movdqu [RAX], xmm2                  ;push counter and bit to the output array
    add RCX, 2                          ;increase size of the array
    movdqu [RDI+RCX*8], xmm3            ;push pointers to the array
    add RCX, 2                          ;increase size of the array
    pinsrq xmm1, RAX, 1                 ;load address to upper part of the register

    ;create a new orphan node
    paddq xmm0, xmm2                    ;calcualte weight of the node
    xor RAX, RAX                        ;set 0 in RAX
    movq xmm0, RAX                      ;zero byte
    sub RSP, 16                         ;move stack pointer
    movups [RSP], xmm1                  ;save nodes pointers
    sub RSP, 16                         ;move stack pointer
    movups [RSP], xmm0                  ;save weight of the node

    ;delete used orphan node from array
    pxor xmm2, xmm2
    movdqu [R10], xmm2 
    movdqu [R10+16], xmm2 
    movdqu [R11], xmm2 
    movdqu [R11+16], xmm2 

    xor R8, R8                          ;set iterator to 0
    inc R9                              ;increase size of the orphan node array
    sub RBX, 2
    jne StateOne

endOfProc:

;           load key register
;#-------------------------------------#
    mov RSP, stackPointer
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
    pop RBX
    pop RSP
    

    ret                                 ;end

makeHuffmanCodeTree endp



end