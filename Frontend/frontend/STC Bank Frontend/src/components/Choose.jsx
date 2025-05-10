import React from "react";
import budgeting from "../images/icon-budgeting.svg";
import onboarding from "../images/icon-onboarding.svg";
import online from "../images/icon-online.svg";
import api from "../images/icon-api.svg";

const Choose = () => {
  return (
    <section className="choose">
      <h2 className="choose-heading">Why choose STC?</h2>
      <p className="choose-text">
        We leverage Open Banking to turn your bank account into your financial
        hub. Control your finances like never before.
      </p>
      <div className="cards-choose">
        <div className="card-choose">
          <img src={online} alt="online icon" />
          <h3>Online Banking</h3>
          <p className="small-choose">
            Our modern web and mobile applications allow you to keep track of
            your finances wherever you are in the world.
          </p>
        </div>
        <div className="card-choose">
          <img src={budgeting} alt="budgeting icon" />
          <h3>Simple Budgeting</h3>
          <p className="small-choose">
            See exactly where your money goes each month. Receive notifications
            when you’re close to hitting your limits.
          </p>
        </div>
        <div className="card-choose">
          <img src={onboarding} alt="onboarding icon" />
          <h3>Fast Onboarding</h3>
          <p className="small-choose">
            We don’t do branches. Open your account in minutes online and start
            taking control of your finances right away.
          </p>
        </div>
        <div className="card-choose">
          <img src={api} alt="api icon" />
          <h3>Open API</h3>
          <p className="small-choose">
            Manage your savings, investments, pension, and much more from one
            account. Tracking your money has never been easier.
          </p>
        </div>
      </div>
    </section>
  );
};

export default Choose;
