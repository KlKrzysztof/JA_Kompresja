;Asembly Language - Project
;Krzyszkof Klecha
;section 11
;semester 5
;year 2024/25
;Huffman coding compresion
;version 0.2
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

compareNodes MACRO node1, node2, result
    movdqu xmm5, node2
    Psrldq xmm5, 8
    movdqu xmm6, node1
    Psrldq xmm6, 8                      ;change order of data from orphan node
    pcmpgtq xmm6, xmm5                  ;check if orphan node is less than greater value from input array
    movq result, xmm6                   ;load mask to register
ENDM

equalNodes MACRO node1, node2, result
    movdqu xmm5, node1
    Psrldq xmm5, 8                      ;change order of data from orphan node
    pcmpeqq xmm5, node2
    movq result, xmm5
ENDM

                            ;RAX,       AL,     R10,    xmm8,     xmm2,  startPipelineTwo, skipToPipelineTwo, pipelineAdress1,    1
codeCreatorPipeline MACRO sourceReg, testReg, codeReg, maskReg, sourceXmm, nextPipeline,     skipPipeline,       variable,    deleteMask
LOCAL continuePipeline
LOCAL pipeline
    mov variable, sourceReg             ;relese register by pushing address on stack
    movaps maskReg, sourceXmm
    pcmpeqw maskReg, xmm6               ;check whitch value is null
pipeline:
    movq RBX, maskReg                      
    psrldq maskReg, 16                  ;move register to next value
    cmp EBX, 65535                      ;if loaded value is not null
    jne continuePipeline                ;then concat a bit to code
    mov sourceReg, R9                   ;else move mask to RAX
    sub sourceReg, deleteMask           ;and subtract 1 from mask for easy deleting less significant 1
    and R9, sourceReg                   ;delete all unnecessary 1
    jmp nextPipeline                    ;else code next byte

continuePipeline:
    psubb sourceXmm, xmm7               ;subtract 48 from code to get raw numeric value
    movq sourceReg, sourceXmm           ;get result to RAX
    
    test testReg, testReg               ;if byte is zero 
    jz skipPipeline                     ;then skip to next pipeline
    inc codeReg                         ;else change LSB on 1 
    ror codeReg, 1                      ;and move to MSB
    jmp nextPipeline
ENDM

reloadPipeline MACRO nullMask, addressSource, addressMem, registerBroker, registerPart, registerQuarter, nextChar
LOCAL nextByte
    movq RBX, nullMask                  ;move null character mask to mask register
    test BX, BX                         ;if character is not null then set pointer on it for pipeline
    jnz nextByte                         ;else load code of next byte for pipeline
    psrldq nullMask, 16                 ;move register to next value
    mov addressSource, addressMem
    add addressSource, 2               ;set pointer to character                  //16 -> 2?
    jmp nextChar                        ;move to next pipeline

nextByte:
    vpsrlq ymm14, ymm14, registerQuarter
    vpextrb addressSource, registerBroker, registerPart      ;get byte from file to pipeline 1
    ;bit shift a quarter of the register
    vpand ymm15, ymm0, ymm14
    vpsrlq ymm15, ymm15, 8
    vpandn ymm0, ymm14, ymm0
    vpor ymm0, ymm0, ymm15        
    vpsllq ymm14, ymm14, registerQuarter
ENDM

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
;R9 - loop counter
makeSortedArray proc stackPointer: QWORD   ;variable counting loops over counting table (0 - 255)

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

;   register preparation
;#-------------------------------------#

    xor R9, R9                  ;set loops to 0
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
    cmp RDX, -1
    je endProc
    mov [RSI+RBX*8], RDX                ;save the byte
    mov RAX, [RDI+RDX*8]                ;load its counter
    inc RBX                             ;increment the size of the array
    mov [RSI+RBX*8], RAX                ;save a counter
    inc RBX                             ;increment the size of the array

    mov R10, RAX                        ;set previous counter as current one

    xor RCX, RCX                        ;reset iterator
    inc R9                              ;increase counter
    cmp R9, 255                         ;check if all array was searched
    jle searchForMin                     ;if not search again

;           load key register
;#-------------------------------------#
endProc:
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
;R12 - counter of saved orphan nodes
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
    movdqu xmm4, [RSP+R8*8]             ;else load  from orphan nodes
    add RBX, 2                          ;increment input array iterator
    jmp goToStateTwo                ;and to to state 2
continueStateOne:
    movdqu xmm2, [RSI+RBX*8]            ;load next byte and counter
    add RBX, 2                          ;increment input array iterator
    cmp R9, R8                          ;check if orphan nodes array ends
    je endOfStateOne                    ;true: create 2 nodes and connect it by parent node
searchForValues:
    movdqu xmm4, [RSP+R8*8]             ;load orphan node
    compareNodes xmm2, xmm4, RAX        ;compare which node is greater, and save mask in RAX
    test RAX, RAX                       ;check result of comparison
    jnz goToStateTwo                     ;if result is "less than" than go to state 2 
    add R8, 4                           ;else move iterator to next orphan node
    cmp R9, R8                          ;check if orphan nodes array ends
    je endOfStateOne                    ;true: create 2 nodes and connect it by parent node
    jmp searchForValues                 ;continue searching

; State 2: One orphan node is less than 
;   greater value in nodes from input 
;                 array
;#-------------------------------------#

stateTwo:
    cmp R9, R8                          ;check if orphan nodes array ends
    je endOfStateTwo                    ;true: create 1 node and connect it with orphan by parent node
    movdqu xmm4, [RSP+R8*8]             ;load orphan node
    movdqu xmm5, xmm4
    Psrldq xmm5, 8
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
    compareNodes xmm0, xmm4, RAX        ;compare which node is greater, and save mask in RAX
    test RAX, RAX                       ;check result of comparison
    jnz goToStateThree                   ;if result is "less than" than go to state 2 

    compareNodes xmm2, xmm4, RAX        ;compare which node is greater, and save mask in RAX
    test RAX, RAX                       ;check result of comparison
    jnz loadNewNode                      ;if node is less than current than change node

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
    add R8, 4                           ;increase the iterator of orphan nodes array
    cmp R9, R8                          ;check if orphan nodes array ends
    je endOfStateFour                   ;true: create 1 node and connect it with orphan by parent node
    movdqu xmm4, [RSP+R8*8]             ;load orphan node
    compareNodes xmm0, xmm4, RAX        ;compare which node is greater, and save mask in RAX
    movq RAX, xmm6                      ;load mask to register
    test RAX, RAX                       ;check result of comparison
    jnz goToStateFive                    ;if true than exists nodes which are less than values from input array, so find 2 minimal nodes
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
    movdqu xmm4, [RSP+R8*8]             ;load orphan node                           //Tu wystêpuje czasem wyj¹tek (?)
    add R8, 4                           ;increase the iterator of orphan nodes array
    pxor xmm5, xmm5
    equalNodes xmm2, xmm5, RAX
    test RAX, RAX
    jnz stateFive 


;continueStateFive:
    compareNodes xmm2, xmm0, RAX        ;compare which node is greater, and save mask in RAX

    test RAX, RAX
    jz xmm0Greater
    compareNodes xmm2, xmm4, RAX        ;compare which node is greater, and save mask in RAX
    test RAX, RAX                       ;check result of comparison
    jz stateFive
    movdqu xmm2, xmm4
    jmp stateFive

xmm0Greater:
    compareNodes xmm0, xmm4, RAX        ;compare which node is greater, and save mask in RAX
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
    makeNode xmm0, xmm1                 ;macro which write node from xmm0 and xmm1 to output array
    movq xmm1, RAX                      ;load address to lower part of the register

    ;Node 2
    makeNode xmm2, xmm3                 ;macro which write node from xmm2 and xmm3 to output array
    pinsrq xmm1, RAX, 1                 ;load address to upper part of the register

    ;create an orphan node
    paddq xmm0, xmm2                    ;calcualte weight of the node
    xor RAX, RAX                        ;set 0 in RAX
    pinsrq xmm0, RAX, 0                 ;zero byte
    sub RSP, 16                         ;move stack pointer
    movups [RSP], xmm1                  ;save nodes pointers
    sub RSP, 16                         ;move stack pointer
    movups [RSP], xmm0                  ;save weight of the node

    
    add R9, 4                              ;increase size of the orphan node array
    xor R8, R8;                         set iterator to 0
    cmp RDX, RBX                        ;check end of input array
    jne StateOne
    jmp joinBranchesOfTree

;State 2 and 4: save one new node and 
;            an orphan one
;#-------------------------------------#

endOfStateTwo:
endOfStateFour:
    ;set nullptrs in leave
    pxor xmm1, xmm1

    ;Node 1
    makeNode xmm0, xmm1                 ;macro which write node from xmm0 and xmm1 to output array
    movq xmm1, RAX                      ;load address to lower part of the register

    ;Write an orphan node to the output array
    makeNode xmm2, xmm3                 ;macro which write node from xmm2 and xmm3 to output array
    pinsrq xmm1, RAX, 1                 ;load address to upper part of the register

    ;create an orphan node
    paddq xmm0, xmm2                    ;calcualte weight of the node
    xor RAX, RAX                        ;set 0 in RAX
    pinsrq xmm0, RAX, 0                 ;zero byte
    sub RSP, 16                         ;move stack pointer
    movups [RSP], xmm1                  ;save nodes pointers
    sub RSP, 16                         ;move stack pointer
    movups [RSP], xmm0                  ;save weight of the node

    ;delete used orphan node from array
    pxor xmm2, xmm2
    movdqu [R10], xmm2 
    movdqu [R10+16], xmm2 

    add R12, 4                          ;increase counter of saved nodes
    add R9, 4                              ;increase size of the orphan node array
    sub RBX, 2
    xor R8, R8;                         set iterator to 0
    cmp RDX, RBX                        ;check end of input array
    je joinBranchesOfTree
    jmp stateOne

;    State 5: save two orphan nodes
;#-------------------------------------#

endOfStateFive:
    ;Write an orphan node 1 to the output array
    makeNode xmm0, xmm1                 ;macro which write node from xmm0 and xmm1 to output array
    movq xmm1, RAX                      ;load address to lower part of the register

    ;Write an orphan node 2 to the output array
    makeNode xmm2, xmm3                 ;macro which write node from xmm2 and xmm3 to output array
    pinsrq xmm1, RAX, 1                 ;load address to upper part of the register

    ;create a new orphan node
    paddq xmm0, xmm2                    ;calcualte weight of the node
    xor RAX, RAX                        ;set 0 in RAX
    pinsrq xmm0, RAX, 0                 ;zero byte
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

    add R12, 8                          ;increase counter of saved nodes
    xor R8, R8                          ;set iterator to 0
    add R9, 4                              ;increase size of the orphan node array
    sub RBX, 4
    cmp RDX, RBX                        ;check end of input array
    je joinBranchesOfTree
    jmp stateOne

joinBranchesOfTree:
    add R12, 4                          ;add one node to check if only root is an orphan node
    cmp R9, R12                         ;check if orphan nodes ends
    je endOfProc                        ;true: end procedure
    sub R12, 4                          ;else join rest of oprhan nodes
    pxor xmm4, xmm4                     ;set 0 in xmm

    ;load orphan nodes
    movdqu xmm0, [RSP]
    movdqu xmm5, xmm0
    Psrldq xmm5, 8                      ;change order of data from orphan node
    pcmpeqq xmm5, xmm4
    movq RAX, xmm5
    test RAX, RAX
    jz continueJoining                 ;if node is not null then load next node
    add RSP, 32                         ;else go to next node and check again
    sub R9, 4
    sub R12, 4
    jmp joinBranchesOfTree

continueJoining:
    movdqu xmm1, [RSP+16]               ;load pointers of that node
    mov R10, RSP                        ;remember address of orphan node to erase if it will be write
notNullNodeSearch:
    add R8, 4                           ;go to next orphan node
    lea R11, [RSP+R8*8]
    movdqu xmm2, [R11]                  ;load the node
    equalNodes xmm2, xmm4, RAX
    test RAX, RAX
    jnz notNullNodeSearch                ;if node is null then go to next node
    add R8, 2
    movdqu xmm3, [RSP+R8*8]             ;load pointers of the node
    add R8, 2
    add RBX, 4
    jmp stateFive                       ;go to state 5

endOfProc:
    
    makeNode xmm0, xmm1                 ;macro which write node from xmm2 and xmm3 to output array
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

;pointers to loaded code:
;   RAX     - pipeline 1
;   R8      - pipeline 2
;   R14     - pipeline 3
;   R15     - pipeline 4
;EBX    - mask of character is not null
;RCX    - code array
;RDX    - length of a file
;R9     - data mask for pipelines 
;creating block of code:
;   R10     - pipeline 1
;   R11     - pipeline 2
;   R12     - pipeline 3
;   R13     - pipeline 4
;RDI    - file array
;RSI    - coded array
;ymm0   - bytes of file
;ymm1   - coded bytes
;xmm12  - for extracting bytes from ymm0
;xmm13  - for register drop
;ymm14  - mask for bit shifting only a quarter of ymm register
;ymm15  - for extracting bits from file to make a bit shift
;code character storage:
;   xmm2    - pipeline 1
;   xmm3    - pipeline 2
;   xmm4    - pipeline 3
;   xmm5    - pipeline 4
;xmm6   - zero for detecting end of code
;xmm7   - for subtracting from character to detect '0' 
;for mask storaging:
;   xmm8    - pipeline 1
;   xmm9    - pipeline 2
;   xmm10   - pipeline 3
;   xmm11   - pipeline 4
codeFile proc ;procedura do debugowania bo coœ nie dzia³a

    LOCAL stackPointer: QWORD 
    LOCAL zeroChars: DWORD
    LOCAL pipelineAdress1: QWORD
    LOCAL pipelineAdress2: QWORD
    LOCAL pipelineAdress3: QWORD
    LOCAL pipelineAdress4: QWORD
    LOCAL loopCounter: DWORD
              

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

    ;mov quarterBitShift, 9FFFFFFFh
    xor RAX, RAX
    mov loopCounter, EAX
    not RAX
    vxorps ymm14, ymm14, ymm14
    movq xmm14, RAX
    ;movd xmm14, quarterBitShift         ;load mask for quarter bit shifting
    mov zeroChars, 808464432            ;vector of four '0' characters
    mov RDI, RDX                        ;keep file array pointer in RDI
    mov RSI, R8                         ;keep coded array pointer in RSI
    mov RDX, R9                         ;keep file length in RDX
    vxorps ymm1, ymm1, ymm1             ;clear register
    xor R9, R9
    xor R10, R10
    xor R11, R11
    xor R12, R12
    xor R13, R13
    xor RBX, RBX
    pxor xmm6, xmm6
    ;pxor mm1, mm1
    ;movd mm1, 1
    ;psrlq mm1, 64
    pxor xmm7, xmm7
    movd xmm7, zeroChars

loadFrame:
    add RDI, R9                         ;move file pointer to next frame
    sub RDX, R9                         ;shorten length of the array by length of the frame
    xor R9, R9                          ;reset iterator
    cmp RDX, R9                         ;if iterator != array length
    je endCoding                        ;then end procedure
    vmovdqu ymm0, ymmword ptr[RDI]      ;load file to register
    vextracti128 xmm12, ymm0, 1
    ;creating mask for pipelines
    cmp RDX, 32                         ;compare if file length is greater than length of frame then set all 32 bit as 1
    jle lessMask                        ;else set n bits mask
    sub R9D, 1                          ;all bits change on 1
lessMask:
    mov R9D, 1                          
    shlx R9D, R9D, EDX                        ;bit shift n times
    sub R9D, 1                          ;subtract 1 so lesser bits change on 1
    
nextByte:    
    vpextrb RAX, xmm0, 0                ;get byte from file to pipeline 1
    vpextrb R8, xmm0, 8                 ;get byte from file to pipeline 2
    vpextrb R14, xmm12, 0               ;get byte from file to pipeline 3
    vpextrb R15, xmm12, 8               ;get byte from file to pipeline 4
    vpsrlq ymm0, ymm0, 8                ;bit shift register to get next byte
    vpsrlq ymm12, ymm12, 8              ;bit shift register to get next byte
    ;load pointers to code to the register
    mov RAX, [RCX+RAX*8]                
    mov R8, [RCX+R8*8]
    mov R14, [RCX+R14*8]
    mov R15, [RCX+R15*8]
loadCode:
    ;load value of code to pipelines
    movd xmm2, dword ptr [RAX]                      
    test R8, R8
    jz startPipeline
    movd xmm3, dword ptr [R8]
    test R14, R14
    jz startPipeline
    movd xmm4, dword ptr [R14]
    test R15, R15
    jz startPipeline
    movd xmm5, dword ptr [R15]
    
    ;jeœli w rejestrze skoñczy³y siê dane wtedy zrzucamy dane do pamiêci w znane nam miejsce i ustawiamy wskaŸnik na null i opró¿niamy rejestry

startPipeline:
    codeCreatorPipeline RAX, AL, R10, xmm8, xmm2, startPipelineTwo, skipToPipelineTwo, pipelineAdress1, 1

skipToPipelineTwo:
    sar R10, 1   
startPipelineTwo:
    test R8, R8
    jz ReloadPipelines
    codeCreatorPipeline R8, R8b, R11, xmm9, xmm3, startPipelineThree, skipToPipelineThree, pipelineAdress2, 256

skipToPipelineThree:
    sar R11, 1
startPipelineThree:
    test R14, R14
    jz ReloadPipelines
    codeCreatorPipeline R14, R14b, R12, xmm10, xmm4, startPipelineFour, skipToPipelineFour, pipelineAdress3, 512
    
skipToPipelineFour:
    sar R12, 1
startPipelineFour:
    test R15, R15
    jz ReloadPipelines
    codeCreatorPipeline R15, R15b, R13, xmm11, xmm5, ReloadPipelines, ReloadPipelines, pipelineAdress4, 1024
    
;Decisive block for pipelines wether to 
;   still code or load next charcter
;---------------------------------------;
ReloadPipelines:
;to na koniec
    

    ;czy opró¿niæ rejestry kodu -> zmieszaæ wskaŸnik i maskê i czy równa siê zero?
    mov EAX, loopCounter
    inc EAX
    mov loopCounter, EAX
    cmp loopCounter, 64                 ;if register is full then drop them to memory
    jne stopRegisterDrop                ;else load next characters
    movq xmm1, R10
    test R8, R8
    jz drop1                            ;check pipelines which of them are working then drop data to memory
    pinsrq xmm1, R11, 1
    test R14, R14
    jz drop1_2
    movq xmm13, R12
    test R15, R15
    jz drop1_3
    pinsrq xmm13, R13, 1                ;if all pipelines are working then merge data and drop to memory

    vinserti128 ymm1, ymm1, xmm13, 1

    vmovdqu ymmword ptr[RSI], ymm1
    add RSI, 32                         ;move ptr to empty space
    jmp stopRegisterDrop


    ;if some pipelines are not working then file is already compressed so drop and quit procedure
drop1_3:
    movups [RSI], xmm1
    add RSI, 16
    mov [RSI], R12
    add RSI, 8
    jmp endCoding


drop1_2:
    movups [RSI], xmm1
    add RSI, 16
    jmp endCoding

drop1:
    mov [RSI], R10
    add RSI, 8
    jmp endCoding

    ;czy wczytaæ kolejny znak kodu
stopRegisterDrop:
;    reloadPipeline xmm8, RAX, pipelineAdress1, xmm0, 0, 0, setPipeline2
    ;przetworzyæ to na makro
    movq RBX, xmm8                      ;move null character mask to mask register
    test BX, BX                         ;if character is not null then set pointer on it for pipeline
    jnz nextByteP1                       ;else load code of next byte for pipeline
    psrldq xmm8, 16                     ;move register to next value
    mov RAX, pipelineAdress1
    add RAX, 2                         ;set pointer to character
    jmp setPipeline2                      ;move to next pipeline

nextByteP1:
vpsrlq ymm14, ymm14, 0
    vpextrb RAX, xmm0, 0                ;get byte from file to pipeline 1
    ;bit shift first quarter of the register
    vpand ymm15, ymm0, ymm14
    vpsrlq ymm15, ymm15, 8
    vpandn ymm0, ymm14, ymm0
    vpor ymm0, ymm0, ymm15              

setPipeline2:
    reloadPipeline xmm9, R8, pipelineAdress2, xmm0, 1, 64, setPipeline3

setPipeline3:
    reloadPipeline xmm10, R14, pipelineAdress3, xmm12, 0, 128, setPipeline4
    vextracti128 xmm12, ymm0, 1

setPipeline4:
    reloadPipeline xmm11, R15, pipelineAdress4, xmm12, 1, 192, loadCode
    vextracti128 xmm12, ymm0, 1

mov R8, R9                         ;copy mask of data
    mov RAX, 00000000000000ffh                        ;set mask for compatison
    and AX, R8w             
    jz loadFrame

endCoding:
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

    ret
codeFile endp


end