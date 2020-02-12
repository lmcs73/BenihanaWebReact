import React from "react";
import styled, { keyframes } from "styled-components";
const BounceAnimation = keyframes`
  0%,
  80%,
  100% {
    -webkit-transform: scale(0);
    -ms-transform: scale(0);
    transform: scale(0);
  }
  40% {
    -webkit-transform: scale(1);
    -ms-transform: scale(1);
    transform: scale(1);
  }
`;
const DotWrapper = styled.div`
  display: flex;
  align-items: flex-end;
  justify-content: center;
  align-content: center;
  height: 40vh;
`;
const Dot = styled.div`
  display: inline-block;
  width: 1rem;
  height: 1rem;
  border-radius: 100%;
  background-color: #ff556e;
  /* Animation */
  animation: ${BounceAnimation} 1.4s infinite ease-in-out both;
  -webkit-animation: ${BounceAnimation} 1.4s infinite ease-in-out both;
  animation-delay: ${props => props.delay};
`;
export const LoadingDots = () => {
  return (
    <DotWrapper>
      <Dot delay="-0.32s" />
      <Dot delay="-0.16s" />
      <Dot delay="0s" />
    </DotWrapper>
  );
};
