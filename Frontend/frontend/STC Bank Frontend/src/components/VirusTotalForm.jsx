import React, { useState } from 'react';
import axios from 'axios';
import './VirusTotal.css';

const VirusTotalForm = () => {
  const [file, setFile] = useState(null);
  const [response, setResponse] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleFileChange = (e) => {
    setFile(e.target.files[0]);
    setResponse(null);
    setError(null);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!file) {
      setError('Please select a file first');
      return;
    }

    setLoading(true);
    setError(null);

    const formData = new FormData();
    formData.append('file', file);

    try {
      const response = await axios.post('https://localhost:7152/api/virustotal/upload', formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });
      setResponse(response.data);
    } catch (err) {
      setError(err.response?.data?.message || 'An error occurred while uploading the file');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="virustotal-container">
      <h2>VirusTotal File Scanner</h2>
      <form onSubmit={handleSubmit} className="virustotal-form">
        <div className="file-input-container">
          <input
            type="file"
            onChange={handleFileChange}
            className="file-input"
            id="file-input"
          />
          <label htmlFor="file-input" className="file-input-label">
            {file ? file.name : 'Choose a file'}
          </label>
        </div>
        <button type="submit" className="submit-button" disabled={loading}>
          {loading ? 'Scanning...' : 'Scan File'}
        </button>
      </form>

      {error && <div className="error-message">{error}</div>}

      {response && (
        <div className="response-container">
          <h3>Scan Results:</h3>
          <div className="response-details">
            <p><strong>Hash:</strong> {response.hash}</p>
            <p><strong>Status:</strong> {response.status}</p>
            <p><strong>File URL:</strong> <a href={response.file_url} target="_blank" rel="noopener noreferrer">{response.file_url}</a></p>
            <p><strong>Message:</strong> {response.message}</p>
          </div>
        </div>
      )}
    </div>
  );
};

export default VirusTotalForm; 