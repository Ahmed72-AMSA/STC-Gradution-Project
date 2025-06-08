import React from "react";

function InputField({ classes, type, id, placeholder, label, value, disabled, onChangeF }) {
  return (
    <div className={`form-group mb-3 ${classes}`}>
      <label htmlFor={id} className="form-label">
        {label}
      </label>
      <input
        type={type}
        id={id}
        className="form-control"
        placeholder={placeholder}
        value={value}
        disabled={disabled}
        onChange={onChangeF}
      />
    </div>
  );
}

export default InputField;
