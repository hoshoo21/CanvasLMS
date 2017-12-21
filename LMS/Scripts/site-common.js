var Common = {
    CheckIsNull: function (_value) {
        try {
            _value = _value.toString();
            _value = _value.trim();
            return ((_value == null || typeof _value == 'undefined' || _value == 'null' || _value.length == 0) ? true : false);
        } catch (e) {
            return true;
        }
    },

    CheckIsNullAndReplace: function (_value, _replace) {
        return !Common.CheckIsNull(_value) ? _value : _replace;
    },
}