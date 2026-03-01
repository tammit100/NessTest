import { HttpInterceptorFn } from '@angular/common/http';

export const apiInterceptor: HttpInterceptorFn = (req, next) => {
  const baseUrl = 'https://localhost:7237/api';

  // אם הבקשה כבר מכילה http (למשל קריאה לנכס חיצוני), אל תיגע בה
  if (req.url.startsWith('http')) {
    return next(req);
  }

  // הזרקת ה-Base URL לכל בקשה
  const apiReq = req.clone({
    url: `${baseUrl}/${req.url}`
  });

  return next(apiReq);
};
