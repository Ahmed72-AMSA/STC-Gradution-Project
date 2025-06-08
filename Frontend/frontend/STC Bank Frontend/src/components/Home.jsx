import React from "react";
import Choose from "./Choose";
import Hero from "./Hero";
import ChatBot from "./Chatbot";
import Header from "./Header";
import './Home.css'
import ZakatCalculator from "./zakahCalculator";
import Partners from "./partners";
import Reports from "./Reports";

const Home = () => {
  return (
    <>
      <Header/>
      <Hero />
      <ChatBot/>
      <Choose />
      <ZakatCalculator/>
      <Partners />
      <Reports/>
    </>
  );
};

export default Home;
