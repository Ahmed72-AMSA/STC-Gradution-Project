import React, { useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Container, Row, Col, Form, Button, Alert } from 'react-bootstrap';
import axios from 'axios';
import './phishing.css';

const PhishingURLForm = () => {
  const [url, setUrl] = useState('');
  const [manualInputs, setManualInputs] = useState({
    DNS_Record: '',
    Web_Traffic: '',
    Domain_Age: '',
    iFrame: '',
    Mouse_Over: '',
    Right_Click: '',
    Web_Forwards: '',
  });
  const [predictionResult, setPredictionResult] = useState(null);
  const [features, setFeatures] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleChange = (e) => {
    setUrl(e.target.value);
  };

  const handleManualChange = (e) => {
    const { name, value } = e.target;
    setManualInputs({ ...manualInputs, [name]: value });
  };

  const extractFeaturesFromURL = (url) => {
    let urlObj;
    try {
      urlObj = new URL(url);
    } catch (error) {
      return null;
    }

    const domainEnd = urlObj.hostname.split('.').pop();
    const famousTLDs = ['com', 'yahoo', 'edu', 'eg', 'net', 'org'];
    const domainEndFlag = famousTLDs.includes(domainEnd) ? 0 : 1;

    return {
      Have_IP: urlObj.hostname.match(/^(\d{1,3}\.){3}\d{1,3}$/) ? 1 : 0,
      Have_At: url.includes('@') ? 1 : 0,
      URL_Length: url.length,
      URL_Depth: urlObj.pathname.split('/').filter(seg => seg).length,
      Redirection: url.includes('//') ? 1 : 0,
      https_Domain: urlObj.protocol === 'https:' ? 1 : 0,
      TinyURL: url.length < 30 ? 1 : 0,
      Prefix_Suffix: urlObj.hostname.includes('-') ? 1 : 0,
      Domain_End: domainEndFlag,
    };
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setPredictionResult(null);
    setFeatures(null);

    const extracted = extractFeaturesFromURL(url);
    if (!extracted) {
      setError("Invalid URL. Please enter a valid one.");
      setLoading(false);
      return;
    }

    const manualFields = {};
    for (const key in manualInputs) {
      if (manualInputs[key] === '') {
        setError(`Please fill in the "${key}" field.`);
        setLoading(false);
        return;
      }
      manualFields[key] = parseInt(manualInputs[key]);
    }

    const finalFeatures = { ...extracted, ...manualFields };
    setFeatures(finalFeatures);

    try {
      const response = await axios.post('http://127.0.0.1:8003/predict', finalFeatures);
      setPredictionResult(response.data);
    } catch (err) {
      console.error(err);
      setError("Could not connect to prediction API.");
    }

    setLoading(false);
  };

  return (
    <div className="phishing-form-wrapper">
      <Container>
        <h2 className="phishing-header mb-4">Phishing URL Detector</h2>
        <Row className="justify-content-center">
          <Col md={12}>
            <Form onSubmit={handleSubmit} className="phishing-form-container">
              <Form.Group className="mb-3">
                <Form.Label className="phishing-label">Enter URL</Form.Label>
                <Form.Control
                  type="text"
                  placeholder="https://example.com"
                  value={url}
                  onChange={handleChange}
                  className="phishing-input"
                  required
                />
              </Form.Group>

              {Object.entries(manualInputs).map(([field, value]) => (
                <Form.Group key={field} className="mb-3">
                  <Form.Label className="phishing-label">{field}</Form.Label>
                  {field === 'Domain_Age' ? (
                    <Form.Control
                      type="number"
                      name={field}
                      value={value}
                      onChange={handleManualChange}
                      className="phishing-input"
                      required
                    />
                  ) : (
                    <Form.Select
                      name={field}
                      value={value}
                      onChange={handleManualChange}
                      className="phishing-input"
                      required
                    >
                      <option value="">Select...</option>
                      <option value="0">0 (No)</option>
                      <option value="1">1 (Yes)</option>
                    </Form.Select>
                  )}
                </Form.Group>
              ))}

              <Button variant="danger" type="submit" className="w-100 submit-phishing" disabled={loading}>
                {loading ? 'Analyzing...' : 'Check URL'}
              </Button>
            </Form>

            {error && <Alert variant="danger" className="mt-4">{error}</Alert>}

            {predictionResult && (
              <Alert variant={predictionResult === "Malicious URL" ? "danger" : "success"} className="mt-4">
                {predictionResult === "Malicious URL"
                  ? "⚠️ This URL is Malicious (Phishing)"
                  : "✅ This URL is Safe"}
              </Alert>
            )}

            {features && (
              <div className="mt-4 phishing-result">
                <h5>Submitted Features:</h5>
                <ul className="list-group">
                  {Object.entries(features).map(([key, value]) => (
                    <li key={key} className="list-group-item d-flex justify-content-between">
                      <strong>{key}</strong>
                      <span>{value}</span>
                    </li>
                  ))}
                </ul>
              </div>
            )}
          </Col>
        </Row>
      </Container>
    </div>
  );
};

export default PhishingURLForm;
