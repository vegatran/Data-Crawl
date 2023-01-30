function Reset() {
    $('div[name="error"]').addClass('d-none');
    $('div[name="success"]').addClass('d-none');
}

function CheckData(str) {
    if (str.val().length <= 0) {
        $('div[name="error"]').removeClass('d-none');
        $('div[name="error"]').find('p').html('Please input number: ' + str.attr('id'));
        return false;
    }
    return true;
}

function Permutation() {
    let number1 = $("#number1");
    let number2 = $('#number2');

    if (!CheckData(number1))
        return;
    if (!CheckData(number2))
        return;

    if (number1.val().length != number2.val().length) {
        $('div[name="error"]').removeClass('d-none');
        $('div[name="error"]').find('p').html('Not equivalent.');
        $('div[name="success"]').addClass('d-none');
        return ;
    }

    let ch1 = sort(number1.val());
    let ch2 = sort(number2.val());

    if (ch1 != ch2) {
        $('div[name="error"]').removeClass('d-none');
        $('div[name="error"]').find('p').html('Not equivalent.');
        $('div[name="success"]').addClass('d-none');
        return ;
    }

    $('div[name="success"]').removeClass('d-none');
    $('div[name="error"]').addClass('d-none');
    return ;
}

function sort(str) {
    let ch = []
    for (let i = 0; i < str.length; i++) {
        ch.push(str[i]);
    }
    return ch.sort().join('');
}


function TaoSoNgauNhien(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function TaoDanhSachSo() {
    let lstData = [];
    for (let i = 0; i < 500; i++) {
        let a = TaoSoNgauNhien(100, 100000);
        let obj = {};
        obj['key'] = sort(a.toString());
        obj['value'] = a;
        lstData.push(obj);
    }
    $('#lstNumber').html(JSON.stringify(_.pluck(lstData, 'value')));

    let lstResut = [], lstError = [];
    let grouped = _.groupBy(lstData, function (x) {
        return x.key;
    })

    //Lấy ra danh sách data hoán vị
    lstError = _.pick(grouped, function (value, key, object) {
        return value.length > 1;
    });
    var reducError = _.map(lstError, function (x) {
        return _.reduce(x, function (obj, y) {
            return [y.value, y.value];
        }, [, 0])
    })
    let dataError = _.map(reducError, function (num) {
        return num[1];
    });
    $('#lstError').html(JSON.stringify(_.values(dataError)));


    //Lấy data theo yêu cầu
    lstResut = _.pick(grouped, function (value, key, object) {
        return value.length == 1;
    });
    var reducSuccess = _.map(lstResut, function (x) {
        return _.reduce(x, function (obj, y) {
            return [y.value, y.value];
        }, [, 0])
    })
    let dataSuccess = _.map(reducSuccess, function (num) {
        return num[1];
    });

    $('#lstResult').html(JSON.stringify(_.values(dataSuccess)));
    
}
