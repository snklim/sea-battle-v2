/**
 * @param {number[][]} board
 * @return {number}
 */
var movesToChessboard = function(board) {
    var pairs = [];
    for (let i = 0; i < board.length - 1; i++) {
        for (let j = i + 1; j < board.length; j++) {
            pairs.push({t: 'r', i1: i, i2: j});
            pairs.push({t: 'c', i1: i, i2: j});
        }
    }

    var valid1 = [];
    var valid2 = [];
    var val1 = 1;
    var val2 = 0;

    for (let i = 0; i < board.length; i++) {
        valid1.push([]);
        valid2.push([]);
        for (let j = 0; j < board[i].length; j++) {
            valid1[i].push(val1);
            valid2[i].push(val2);
            let val = val1;
            val1 = val2;
            val2 = val;
        }
        if (board[i].length % 2 === 0) {
            let val = val1;
            val1 = val2;
            val2 = val;
        }
    }

    if (isValid(board)) {
        return 0;
    }

    var minSwapsNumber = -1;

    var time = new Date().getTime();

    var maxSwapNumbers = Math.min(5, board.length - 1);

    getSwapsList(pairs.length, maxSwapNumbers,
        (index, swapNumbers, swaps) => {
            swap(board, pairs[index]);

            if (areSame(board, valid1) || areSame(board, valid2)) {
                console.log(new Date().getTime() - time, swaps);

                if (minSwapsNumber === -1 || minSwapsNumber > swapNumbers) {
                    minSwapsNumber = swapNumbers;
                }

                return true;
            }

            return new Date().getTime() - time >= 5000;
        },
        (index) => {
            swap(board, pairs[index]);
        });

    console.log(new Date().getTime() - time);

    return minSwapsNumber;
}

var areSame = function (board1, board2) {
    for (var i = 0; i < board1.length; i++) {
        for (var j = 0; j < board1[i].length; j++) {
            if (board1[i][j] !== board2[i][j]) return false;
        }
    }
    return true;
}

var getSwapsList = function(length,swapsNumber,doSwap,undoSwap) {
    return swapsListRecursion(length, swapsNumber, doSwap, undoSwap, 1, {threshold:0},[]);
}

var swapsListRecursion = function(length,swapsNumber,doSwap, undoSwap, level,ctx,swaps) {
    for (var i = 0; i < length; i++) {
        if (swaps.length > 0 && swaps[swaps.length - 1] >= i) {
            continue;
        }
        swaps.push(i)
        if (doSwap(i, level, swaps)) {
            ctx.threshold++;
            undoSwap(i);
            swaps.length -= 1;
            break;
        }
        if (swapsNumber - 1 > ctx.threshold) {
            swapsListRecursion(length, swapsNumber - 1, doSwap, undoSwap, level + 1, ctx, swaps);
        }
        swaps.length -= 1;
        undoSwap(i);
    }
}

var swap = function(board,pair) {
    if (pair.t === 'r') {
        swapRows(board, pair.i1, pair.i2);
    } else if (pair.t === 'c') {
        swapColumns(board, pair.i1, pair.i2);
    }
}

var swapColumns = function(board,y1,y2) {
    for (var x = 0; x < board.length; x++) {
        var val = board[x][y1];
        board[x][y1] = board[x][y2];
        board[x][y2] = val;
    }
}

var swapRows = function(board,x1,x2) {
    for (var y = 0; y < board[x1].length; y++) {
        var val = board[x1][y];
        board[x1][y] = board[x2][y];
        board[x2][y] = val;
    }
}

var isValid = function(board) {
    var valid = true;
    for (var x = 0; x < board.length; x++) {
        for (var y = 0; y < board[x].length; y++) {
            valid = valid && isValidNeighbours(board, x, y);
        }
    }
    return valid;
}

var isValidNeighbours = function(board,x,y) {
    var value = board[x][y];
    var oppositeValue = value === 1 ? 0 : 1;
    var valid = true;
    [{x: x - 1, y: y}, {x: x, y: y + 1}, {x: x + 1, y: y}, {x: x, y: y - 1}].forEach(m => {
        if (0 <= m.x && m.x < board.length
            && 0 <= m.y && m.y < board[m.x].length)
            valid = valid && board[m.x][m.y] === oppositeValue;
    });
    return valid;
}

var test1 = [[0,0,1,1,1,0,1],[0,0,1,1,1,0,1],[0,0,1,1,1,0,1],[1,1,0,0,0,1,1],[1,1,0,0,0,1,1],
    [0,0,1,1,1,0,1],[1,1,0,0,0,1,1]];//-1

var test2 = [[0,1,1,0],[0,1,1,0],[1,0,0,1],[1,0,0,1]];//2

var test3 = [[1,0,1,1,1,1,0,0,0],[0,1,0,0,0,0,1,1,1],[1,0,1,1,1,1,0,0,0],[0,1,0,0,0,0,1,1,1],
    [1,0,1,1,1,1,0,0,0],[0,1,0,0,0,0,1,1,1],[0,1,0,0,0,0,1,1,1],[1,0,1,1,1,1,0,0,0],[0,1,0,0,0,0,1,1,1]];//5

var test4 = [[1,1,0],[0,0,1],[0,0,1]];//2

var test5 = [[0,1,0,1,0],[1,0,1,0,1],[1,0,1,0,1],[0,1,0,1,0],[1,0,1,0,1]];//1

var test6 = [[0,1,1,0,0,1,1,1,0],[0,1,1,1,0,0,1,0,1],[0,1,1,0,0,1,1,1,0],[0,1,1,0,0,1,1,1,0],
    [0,1,1,1,0,0,1,0,1],[0,1,1,0,0,1,1,1,0],[0,1,1,1,0,0,1,0,1],[0,1,1,0,0,1,1,1,0],[0,1,1,1,0,0,1,0,1]]

var test7 = [[0,1,0,0,1,1,1,0,0,1],[1,0,1,1,0,0,0,1,1,0],[0,1,0,0,1,1,1,0,0,1],[0,1,0,0,1,1,1,0,0,1],
    [1,0,1,1,0,0,0,1,1,0],[1,0,1,1,0,0,0,1,1,0],[1,0,1,1,0,0,0,1,1,0],[1,0,1,1,0,0,0,1,1,0],
    [0,1,0,0,1,1,1,0,0,1],[0,1,0,0,1,1,1,0,0,1]];

var test8 = [[1,0,1,0,1,0,0,0,1,1,0],[1,0,1,0,1,0,0,0,1,1,0],[0,1,0,1,0,1,1,1,0,0,1],[0,1,0,1,0,1,1,1,0,0,1],
    [1,0,1,0,1,0,0,0,1,1,0],[0,1,0,1,0,1,1,1,0,0,1],[0,1,0,1,0,1,1,1,0,0,1],[1,0,1,0,1,0,0,0,1,1,0],
    [1,0,1,0,1,0,0,0,1,1,0],[1,0,1,0,1,0,0,0,1,1,0],[0,1,0,1,0,1,1,1,0,0,1]]

console.log(movesToChessboard(test8));