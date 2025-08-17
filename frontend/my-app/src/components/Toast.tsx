import React, { useEffect, useState } from 'react';

export interface ToastProps {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message?: string;
  duration?: number;
  onClose: (id: string) => void;
}

const Toast: React.FC<ToastProps> = ({ 
  id, 
  type, 
  title, 
  message, 
  duration = 5000, 
  onClose 
}) => {
  const [isVisible, setIsVisible] = useState(true);
  const [isLeaving, setIsLeaving] = useState(false);

  useEffect(() => {
    const timer = setTimeout(() => {
      handleClose();
    }, duration);

    return () => clearTimeout(timer);
  }, [duration]);

  const handleClose = () => {
    setIsLeaving(true);
    setTimeout(() => {
      setIsVisible(false);
      onClose(id);
    }, 300);
  };

  const getTypeStyles = () => {
    switch (type) {
      case 'success':
        return {
          bg: 'bg-green-50 dark:bg-green-900/20',
          border: 'border-green-200 dark:border-green-800',
          icon: '✅',
          iconColor: 'text-green-500',
          titleColor: 'text-green-800 dark:text-green-200',
          messageColor: 'text-green-600 dark:text-green-300'
        };
      case 'error':
        return {
          bg: 'bg-red-50 dark:bg-red-900/20',
          border: 'border-red-200 dark:border-red-800',
          icon: '❌',
          iconColor: 'text-red-500',
          titleColor: 'text-red-800 dark:text-red-200',
          messageColor: 'text-red-600 dark:text-red-300'
        };
      case 'warning':
        return {
          bg: 'bg-yellow-50 dark:bg-yellow-900/20',
          border: 'border-yellow-200 dark:border-yellow-800',
          icon: '⚠️',
          iconColor: 'text-yellow-500',
          titleColor: 'text-yellow-800 dark:text-yellow-200',
          messageColor: 'text-yellow-600 dark:text-yellow-300'
        };
      case 'info':
        return {
          bg: 'bg-blue-50 dark:bg-blue-900/20',
          border: 'border-blue-200 dark:border-blue-800',
          icon: 'ℹ️',
          iconColor: 'text-blue-500',
          titleColor: 'text-blue-800 dark:text-blue-200',
          messageColor: 'text-blue-600 dark:text-blue-300'
        };
    }
  };

  const styles = getTypeStyles();

  if (!isVisible) return null;

  return (
    <div
      className={`transform transition-all duration-300 ease-in-out ${
        isLeaving 
          ? 'translate-x-full opacity-0 scale-95' 
          : 'translate-x-0 opacity-100 scale-100'
      }`}
    >
      <div className={`
        max-w-sm w-full ${styles.bg} ${styles.border} border rounded-lg shadow-lg 
        backdrop-blur-sm hover:shadow-xl transition-shadow duration-300
      `}>
        <div className="p-4">
          <div className="flex items-start">
            <div className={`flex-shrink-0 ${styles.iconColor} text-xl mr-3`}>
              {styles.icon}
            </div>
            <div className="flex-1 min-w-0">
              <h4 className={`text-sm font-semibold ${styles.titleColor} mb-1`}>
                {title}
              </h4>
              {message && (
                <p className={`text-sm ${styles.messageColor} break-words`}>
                  {message}
                </p>
              )}
            </div>
            <button
              onClick={handleClose}
              className="flex-shrink-0 ml-3 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors duration-200"
            >
              <span className="text-lg">×</span>
            </button>
          </div>
        </div>
        
        {/* Progress bar */}
        <div className="h-1 bg-gray-200 dark:bg-gray-700 rounded-b-lg overflow-hidden">
          <div 
            className={`h-full bg-gradient-to-r ${
              type === 'success' ? 'from-green-400 to-green-600' :
              type === 'error' ? 'from-red-400 to-red-600' :
              type === 'warning' ? 'from-yellow-400 to-yellow-600' :
              'from-blue-400 to-blue-600'
            } transition-all duration-300 ease-linear`}
            style={{
              animation: `shrink ${duration}ms linear forwards`
            }}
          />
        </div>
      </div>
    </div>
  );
};

export default Toast;
