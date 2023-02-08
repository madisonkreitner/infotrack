import React from 'react';
import { render, screen } from '@testing-library/react';
import App from './App';

test('renders stastistics page', () => {
  render(<App />);
  const linkElement = screen.getByText('InfoTrack Google Statistics');
  expect(linkElement).toBeInTheDocument();
});
