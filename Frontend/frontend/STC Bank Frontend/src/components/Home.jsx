import React from "react";
import Choose from "./Choose";
import Hero from "./Hero";
import Latest from "./Latest";
import ChatBot from "./Chatbot";
import Header from "./Header";

import './Home.css'
import ZakatCalculator from "./zakahCalculator";
const Home = () => {
  return (
    <>
      <Header/>
      <Hero />
      <ChatBot/>
      <Choose />
      <ZakatCalculator/>
      <Latest />
    </>
  );
};

export default Home;
