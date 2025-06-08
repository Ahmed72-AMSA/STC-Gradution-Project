import React from "react";
import InputField from "./InputField";

function SocialDetails({ data, disabled, onChange }) {
  const keys = Object.keys(data);

  return (
    <div className="row g-3">
      {keys.map((key) => {
        const formattedLabel = key.replace(/([A-Z])/g, " $1").replace(/^./, (str) => str.toUpperCase());
        return (
          <div key={key} className="col-md-6">
            <InputField
              classes="w-100"
              type="text"
              id={key}
              placeholder={`Enter${formattedLabel.toLowerCase()}`}
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

export default SocialDetails;
