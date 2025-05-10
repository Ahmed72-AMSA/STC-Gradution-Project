import React from "react";
import mockups from "../images/image-mockups.png";

const Hero = () => {
  return (
    <section className="hero">
      <div className="hero-desc">
        <h1 className="hero-heading">
          Next generation<span> digital banking</span>{" "}
        </h1>
        <p className="hero-text">
          Take your financial life online. Your STC account will be a
          one-stop-shop for spending, saving, budgeting, investing, and much
          more. Request Invite
        </p>
        <button className="btn-req">Show My Account</button>
      </div>
      <div className="hero-img">
        <img className="phone-img" src={mockups} alt="phone" />
      </div>
    </section>
  );
};

export default Hero;
