import React from 'react';

interface CardProps {
  children: React.ReactNode;
  className?: string;
  variant?: 'default' | 'elevated' | 'outlined';
  header?: React.ReactNode;
  footer?: React.ReactNode;
  padding?: 'none' | 'sm' | 'md' | 'lg';
}

const Card: React.FC<CardProps> = ({
  children,
  className = '',
  variant = 'default',
  header,
  footer,
  padding = 'md',
}) => {
  const baseClasses = 'bg-white rounded-lg border';
  
  const variantClasses = {
    default: 'border-gray-200 shadow-sm',
    elevated: 'border-gray-200 shadow-lg',
    outlined: 'border-gray-300 shadow-none',
  };

  const paddingClasses = {
    none: '',
    sm: 'p-3',
    md: 'p-6',
    lg: 'p-8',
  };

  return (
    <div className={`${baseClasses} ${variantClasses[variant]} ${className}`}>
      {header && (
        <div className="px-6 py-4 border-b border-gray-200">
          {header}
        </div>
      )}
      
      <div className={padding !== 'none' ? paddingClasses[padding] : ''}>
        {children}
      </div>
      
      {footer && (
        <div className="px-6 py-4 border-t border-gray-200 bg-gray-50 rounded-b-lg">
          {footer}
        </div>
      )}
    </div>
  );
};

export default Card;
