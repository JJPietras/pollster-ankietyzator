interface Question {
  questionId: number;
  position: number;
  title: string;
  options: string;
  allowEmpty: boolean;
  maxLength: number;
  type: number;
  answer: any;
}
