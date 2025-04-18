import React, { useState } from 'react';
import "./zakah.css";
import 'bootstrap/dist/css/bootstrap.min.css';


const ZakatCalculator = () => {
  const [inputs, setInputs] = useState({
    moneyValue: "",
    propertyShares: "",
    bondsValue: "",
    profitValue: "",
    gold18Value: "",
    gold21Value: "",
    buildingValue: ""
  });

  const [results, setResults] = useState({
    cashAmount: "",
    propertyAmount: "",
    goldAmount: "",
    buildingAmount: "",
    totalDonations: ""
  });

  const handleInputChange = (e) => {
    const { id, value } = e.target;
    setInputs(prev => ({
      ...prev,
      [id]: value === "" ? "" : Number(value)
    }));
  };

  const calculateDonations = () => {
    const {
      moneyValue = 0,
      propertyShares = 0,
      bondsValue = 0,
      profitValue = 0,
      gold18Value = 0,
      gold21Value = 0,
      buildingValue = 0
    } = inputs;

    const cashAmount = moneyValue * 0.025;
    const propertyAmount = (propertyShares + bondsValue + profitValue) * 0.025;
    const goldAmount = (gold18Value * 2259 + gold21Value * 2635) * 0.025;
    const buildingAmount = buildingValue * 0.025;
    const totalDonations = cashAmount + propertyAmount + goldAmount + buildingAmount;

    setResults({
      cashAmount: cashAmount.toFixed(2),
      propertyAmount: propertyAmount.toFixed(2),
      goldAmount: goldAmount.toFixed(2),
      buildingAmount: buildingAmount.toFixed(2),
      totalDonations: totalDonations.toFixed(2)
    });
  };

  const resetCalculator = () => {
    setInputs({
      moneyValue: "",
      propertyShares: "",
      bondsValue: "",
      profitValue: "",
      gold18Value: "",
      gold21Value: "",
      buildingValue: ""
    });
    setResults({
      cashAmount: "",
      propertyAmount: "",
      goldAmount: "",
      buildingAmount: "",
      totalDonations: ""
    });
  };

  return (
    <div className="zakat-calculator-container">
      <h2 className="calculator-title">Zakat Calculator</h2>
      <p className="calculator-subtitle">Calculate your Zakat obligations according to Islamic principles</p>
      
      <div className="row">
        <div className="col-lg-8">
          {/* Cash Donation Section */}
          <div className="cart-form active">
            <div className="cart-form_header">
              <span>Cash Donation</span>
            </div>
            <div className="cart-form_body">
              <div className="form-group">
                <label htmlFor="moneyValue">Amount of Money</label>
                <div className="input-group">
                  <input
                    type="number"
                    min="0"
                    placeholder="Enter amount"
                    id="moneyValue"
                    value={inputs.moneyValue}
                    onChange={handleInputChange}
                  />
                  <span className="input-suffix">L.E</span>
                </div>
              </div>
            </div>
          </div>

          {/* Properties and Assets Section */}
          <div className="cart-form active">
            <div className="cart-form_header">
              <span>Properties and Assets</span>
            </div>
            <div className="cart-form_body">
              <div className="form-group">
                <label htmlFor="propertyShares">Money in Market</label>
                <div className="input-group">
                  <input
                    type="number"
                    min="0"
                    placeholder="Enter amount"
                    id="propertyShares"
                    value={inputs.propertyShares}
                    onChange={handleInputChange}
                  />
                  <span className="input-suffix">L.E</span>
                </div>
              </div>
              
              <div className="form-group">
                <label htmlFor="bondsValue">Amount of Bonds</label>
                <div className="input-group">
                  <input
                    type="number"
                    min="0"
                    placeholder="Enter amount"
                    id="bondsValue"
                    value={inputs.bondsValue}
                    onChange={handleInputChange}
                  />
                  <span className="input-suffix">L.E</span>
                </div>
              </div>
              
              <div className="form-group">
                <label htmlFor="profitValue">Total Profit</label>
                <div className="input-group">
                  <input
                    type="number"
                    min="0"
                    placeholder="Enter amount"
                    id="profitValue"
                    value={inputs.profitValue}
                    onChange={handleInputChange}
                  />
                  <span className="input-suffix">L.E</span>
                </div>
              </div>
            </div>
          </div>

          {/* Gold Donation Section */}
          <div className="cart-form active">
            <div className="cart-form_header">
              <span>Gold Donation</span>
            </div>
            <div className="cart-form_body">
              <div className="gold-section">
                <div className="gold-input-group">
                  <div className="form-group">
                    <label htmlFor="gold18Value">Gold (18 Karat)</label>
                    <div className="input-group">
                      <input
                        type="number"
                        min="0"
                        placeholder="Grams"
                        id="gold18Value"
                        value={inputs.gold18Value}
                        onChange={handleInputChange}
                      />
                      <span className="input-suffix">g</span>
                    </div>
                  </div>
                  <div className="gold-info">
                    <p className="gold-price">Current value: 2,259 L.E/g</p>
                  </div>
                </div>
                
                <div className="gold-input-group">
                  <div className="form-group">
                    <label htmlFor="gold21Value">Gold (21 Karat)</label>
                    <div className="input-group">
                      <input
                        type="number"
                        min="0"
                        placeholder="Grams"
                        id="gold21Value"
                        value={inputs.gold21Value}
                        onChange={handleInputChange}
                      />
                      <span className="input-suffix">g</span>
                    </div>
                  </div>
                  <div className="gold-info">
                    <p className="gold-price">Current value: 2,635 L.E/g</p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Properties and Buildings Section */}
          <div className="cart-form active">
            <div className="cart-form_header">
              <span>Properties and Buildings</span>
            </div>
            <div className="cart-form_body">
              <div className="form-group">
                <label htmlFor="buildingValue">Monthly Revenue from Buildings</label>
                <div className="input-group">
                  <input
                    type="number"
                    min="0"
                    placeholder="Enter amount"
                    id="buildingValue"
                    value={inputs.buildingValue}
                    onChange={handleInputChange}
                  />
                  <span className="input-suffix">L.E</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Results Section */}
        <div className="col-lg-4">
          <div className="cart-list_total">
            <div className="cart-total_head">
              Zakat Calculation Results
            </div>
            <div className="cart-total_body">
              <div className="result-item">
                <span className="result-label">Cash Zakat:</span>
                <span className="result-value">
                  {results.cashAmount ? `${results.cashAmount} L.E` : "--"}
                </span>
              </div>
              
              <div className="result-item">
                <span className="result-label">Properties Zakat:</span>
                <span className="result-value">
                  {results.propertyAmount ? `${results.propertyAmount} L.E` : "--"}
                </span>
              </div>
              
              <div className="result-item">
                <span className="result-label">Gold Zakat:</span>
                <span className="result-value">
                  {results.goldAmount ? `${results.goldAmount} L.E` : "--"}
                </span>
              </div>
              
              <div className="result-item">
                <span className="result-label">Buildings Zakat:</span>
                <span className="result-value">
                  {results.buildingAmount ? `${results.buildingAmount} L.E` : "--"}
                </span>
              </div>
              
              <div className="divider"></div>
              
              <div className="total-result">
                <span className="total-label">Total Zakat Due:</span>
                <span className="total-value">
                  {results.totalDonations ? `${results.totalDonations} L.E` : "--"}
                </span>
              </div>
            </div>
            
            <div className="action-buttons">
              <button
                className="calculate-btn"
                onClick={calculateDonations}
              >
                Calculate Zakat
              </button>
              <button
                className="reset-btn"
                onClick={resetCalculator}
              >
                Reset
              </button>
            </div>
            
            <div className="zakat-info">
              <p>Zakat is typically 2.5% of your eligible assets held for one lunar year.</p>
              <p>This calculator provides an estimate. Please consult with a scholar for precise calculation.</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ZakatCalculator;