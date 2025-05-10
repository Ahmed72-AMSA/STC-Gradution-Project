import React, { useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Container, Row, Col, Form, Button } from 'react-bootstrap';
import axios from 'axios';
import './LoanForm.css';

const CreditForm = () => {
  const [formData, setFormData] = useState({});
  const [predictionResult, setPredictionResult] = useState(null);
  const [loading, setLoading] = useState(false);

  const fields = [
    { label: "Gender", name: "GENDER", type: "select", options: [{ label: "Male", value: 0 }, { label: "Female", value: 1 }] },
    { label: "Car Owner", name: "Car_Owner", type: "select", options: [{ label: "No", value: 0 }, { label: "Yes", value: 1 }] },
    { label: "Property Owner", name: "Propert_Owner", type: "select", options: [{ label: "No", value: 0 }, { label: "Yes", value: 1 }] },
    { label: "Children", name: "CHILDREN", type: "number" },
    { label: "Annual Income", name: "Annual_income", type: "number" },
    { label: "Type of Income", name: "Type_Income", type: "select", options: [{ label: "Working", value: 1 }, { label: "State servant", value: 2 }, { label: "Commercial associate", value: 3 }, { label: "Pensioner", value: 4 }, { label: "Student", value: 5 }] },
    { label: "Education", name: "EDUCATION", type: "select", options: [{ label: "Lower secondary", value: 0 }, { label: "Higher education", value: 1 }, { label: "Incomplete higher", value: 2 }, { label: "Secondary / secondary special", value: 3 }] },
    { label: "Marital Status", name: "Marital_status", type: "select", options: [{ label: "Single", value: 0 }, { label: "Married", value: 1 }, { label: "Civil marriage", value: 2 }, { label: "Separated", value: 3 }, { label: "Widow", value: 4 }] },
    { label: "Housing Type", name: "Housing_type", type: "select", options: [{ label: "House / Apartment", value: 1 }, { label: "Municipal apartment", value: 2 }, { label: "With parents", value: 3 }, { label: "Co-op apartment", value: 4 }, { label: "Rented apartment", value: 5 }, { label: "Office apartment", value: 6 }] },
    { label: "Employed Days", name: "Employed_days", type: "number" },
    { label: "Mobile Phone", name: "Mobile_phone", type: "select", options: [{ label: "No", value: 0 }, { label: "Yes", value: 1 }] },
    { label: "Work Phone", name: "Work_Phone", type: "select", options: [{ label: "No", value: 0 }, { label: "Yes", value: 1 }] },
    { label: "Phone", name: "Phone", type: "select", options: [{ label: "No", value: 0 }, { label: "Yes", value: 1 }] },
    { label: "Email ID", name: "EMAIL_ID", type: "select", options: [{ label: "No", value: 0 }, { label: "Yes", value: 1 }] },
    { label: "Occupation Type", name: "Type_Occupation", type: "select", options: [{ label: "Laborers", value: 1 }, { label: "Core staff", value: 2 }, { label: "Accountants", value: 3 }, { label: "High skill tech staff", value: 4 }, { label: "Sales staff", value: 5 }, { label: "Managers", value: 6 }, { label: "Drivers", value: 7 }] },
    { label: "Family Members", name: "Family_Members", type: "number" },
  ];

  // Pre-fill any static value fields like Ethnicity
  React.useEffect(() => {
    const initialData = {};
    fields.forEach(field => {
      if (field.value) initialData[field.name] = field.value;
    });
    setFormData(prev => ({ ...initialData, ...prev }));
  }, []);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const response = await axios.post('http://127.0.0.1:8001/predict', formData);
      setPredictionResult(response.data);
    } catch (error) {
      console.error('Error submitting form:', error);
    }
    setLoading(false);
  };

  return (
    <div className="credit-form-wrapper">
      <Container className="credit-form-container mt-5">
        <h2 className='Loan-header'>Loan Approval Predictor</h2>
        <Row>
          <Col sm="12" md="6" className="mx-auto">
            <Form onSubmit={handleSubmit}>
              {fields.map((field) => (
                <Form.Group key={field.name} className="mb-5" controlId={`form${field.name}`}>
                  <Form.Label className="credit-label">{field.label}</Form.Label>

                  {field.type === "select" ? (
                    <Form.Select
                      name={field.name}
                      value={formData[field.name] || ''}
                      onChange={handleChange}
                      className="credit-input"
                      required
                    >
                      <option value="">Select {field.label}</option>
                      {field.options.map((option, index) => (
                        <option key={index} value={option.value}>
                          {option.label}
                        </option>
                      ))}
                    </Form.Select>
                  ) : (
                    <Form.Control
                      type={field.type}
                      name={field.name}
                      value={formData[field.name] || ''}
                      onChange={handleChange}
                      className="credit-input"
                      required
                    />
                  )}
                </Form.Group>
              ))}

              <Button type="submit" className="submit-btn w-100">
                {loading ? 'Submitting...' : 'Submit'}
              </Button>
            </Form>

            {predictionResult && (
              <div className="prediction-result mt-4">
                <h5 className="prediction-title">Prediction Result:</h5>
                <div className="prediction-details">
                  <p className="prediction-text">
                    Based on the information provided, the loan is:
                  </p>
                  <h4 className="loan-amount">{predictionResult}</h4>
                </div>
              </div>
            )}
          </Col>
        </Row>
      </Container>
    </div>
  );
};

export default CreditForm;
