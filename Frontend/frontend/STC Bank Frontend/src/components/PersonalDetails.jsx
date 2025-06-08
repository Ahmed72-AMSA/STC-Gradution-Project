import React from "react";
import InputField from "./InputField";

function PersonalDetails({ data, disabled, onChange }) {
  const keys = Object.keys(data);

  return (
    <div className="row g-3">
      {keys.map((key, index) => {
        const formattedLabel = key.replace(/([A-Z])/g, ' $1').replace(/^./, str => str.toUpperCase());

        return (
          <div key={key} className="col-md-6">
            <InputField
              classes="w-100"
              type="text"
              id={key}
              placeholder={`Enter ${formattedLabel.toLowerCase()}`}
              label={formattedLabel}
              value={data[key]}
              disabled={disabled}
              onChangeF={(e) => onChange(key, e.target.value)}
            />
          </div>
        );
      })}
    </div>
  );
}

export default PersonalDetails;
