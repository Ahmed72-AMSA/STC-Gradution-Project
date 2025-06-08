import React from "react";
import InputField from "./InputField";

function GeneralDetails({ data, disabled, onChange }) {
  return (
    <div className="d-flex flex-column gap-3">
      {Object.keys(data).map((key) => {
        // Format label and placeholder
        const formattedLabel = key.replace(/([A-Z])/g, " $1").replace(/^./, (str) => str.toUpperCase());

        return (
          <InputField
            key={key}
            classes="w-100" // makes input full width
            type="text"
            id={key}
            placeholder={`Enter ${formattedLabel.toLowerCase()}`}
            label={formattedLabel}
            value={data[key]}
            disabled={disabled}
            onChangeF={(e) => onChange(key, e.target.value)}
          />
        );
      })}
    </div>
  );
}

export default GeneralDetails;
