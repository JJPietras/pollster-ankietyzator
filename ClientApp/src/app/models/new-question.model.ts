interface NewQuestion {
    position: number;
    title: string;
    options: string;
    allowEmpty: boolean;
    maxLength: number;
    type: number;
  }
  
interface NewQuestionCreator {
  position: number;
  title: string;
  options: any;
  allowEmpty: boolean;
  maxLength: number;
  type: number;
  helpText: string;
}